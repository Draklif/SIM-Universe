using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class PlayerController : MonoBehaviour
{
    // Movement
    [SerializeField] private float walkingSpeed, thrusterSpeed, jumpSpeed;

    // Mouse
    [SerializeField] private float mouseSensitivity;
    private float xRotation, yRotation, xRotationSmooth, yRotationSmooth, xRotationSmoothR, yRotationSmoothR;

    // Text
    [SerializeField] private TMP_Text textOrbit;
    [SerializeField] private TMP_Text textVelocity;

    // Variables privadas
    private GameObject actualPlanet;
    private Camera cam;
    public PostProcessVolume volume;
    private ChromaticAberration chromaticAberration;

    // Getters y setters
    public Vector3 velocity { get; private set; }
    public Vector3 position { get; private set; }


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cam = GetComponentInChildren<Camera>();
        volume = cam.GetComponent<PostProcessVolume>();
        position = transform.position;
        volume.profile.TryGetSettings(out chromaticAberration);
    }

    private void Update()
    {
        Look();
        Move();
    }

    private void FixedUpdate()
    {
        UpdateActualPlanet();
        UpdatePosition();
        UpdateText();
        chromaticAberration.intensity.value = Mathf.Clamp(Mathf.Max(Mathf.Max(Mathf.Abs(velocity.x), Mathf.Abs(velocity.y)), Mathf.Abs(velocity.z)), 0, 1000) / 1000;
    }

    /// <summary>
    /// Maneja los inputs para el movimiento de rotación (mouse)
    /// </summary>
    private void Look()
    {
        // X = Pitch, Y = Yaw
        // Caputa el desplazamiento del mouse en cada eje
        xRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Evita el gymball lock
        xRotation = Mathf.Clamp(xRotation - Input.GetAxis("Mouse Y") * mouseSensitivity, -40f, 85f);
        yRotation += Input.GetAxis("Mouse X") * mouseSensitivity;

        // Suaviza las rotaciones
        xRotationSmooth = Mathf.SmoothDampAngle(xRotationSmooth, xRotation, ref xRotationSmoothR, 0.1f);
        float yRotationSmooth_Old = yRotationSmooth;
        yRotationSmooth = Mathf.SmoothDampAngle(yRotationSmooth, yRotation, ref yRotationSmoothR, 0.1f);

        // Únicamente mueve la cámara para no afectar normales
        cam.transform.localEulerAngles = Vector3.right * xRotationSmooth;
        // Rota el jugador en el eje Y, suavizando su movimiento
        transform.Rotate(Vector3.up * Mathf.DeltaAngle(yRotationSmooth_Old, yRotationSmooth), Space.Self);
    }

    /// <summary>
    /// Maneja los inputs para el movimiento de desplazamiento
    /// </summary>
    private void Move()
    {
        // Captura el desplazamiento de cada eje
        float horizontalInput = Input.GetAxisRaw("Horizontal") * walkingSpeed;
        float verticalInput = Input.GetAxisRaw("Vertical") * walkingSpeed;

        // Normaliza (retorna un vector unitario que es la dirección del vector) y lo multiplica por la velocidad de movimiento
        Vector3 input = new Vector3(horizontalInput, 0, verticalInput);
        velocity += transform.TransformDirection(input.normalized) * walkingSpeed;

        // --CONTROLES--
        // Ascender desde el planeta en órbita
        if (Input.GetKey(KeyCode.Space))
        {
            velocity += transform.up * thrusterSpeed;
        }
        
        // Descender hacia el planeta en órbita
        if (Input.GetKey(KeyCode.LeftShift))
        {
            velocity -= transform.up * thrusterSpeed;
        }

        // Cancelar movimiento
        if (Input.GetKey(KeyCode.LeftControl)) 
        {
            velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Actualiza el planeta actual orbitado por el jugador
    /// </summary>
    private void UpdateActualPlanet()
    {
        Vector3 forceDirection, acceleration = Vector3.zero, gravityNearest = Vector3.zero;
        float distancePlanet, distanceNearestSurface = float.MaxValue;
        List<GameObject> planets = Universe.Planets;

        // Por cada planeta en la lista de planetas, calcula la aceleración
        foreach (GameObject planet in planets)
        {
            distancePlanet = (planet.GetComponent<Planet>().position - position).sqrMagnitude;
            forceDirection = (planet.GetComponent<Planet>().position - position).normalized;
            acceleration += forceDirection * Universe.gravitationalConstant * planet.GetComponent<Planet>().mass / distancePlanet;
            velocity += acceleration * Time.fixedDeltaTime;

            // Calcula la distancia a la superficie del planeta
            float distanceSurface = Mathf.Sqrt(distancePlanet) - planet.GetComponent<Planet>().radius;

            // Si la distancia a la superficie del planeta es menor que la distancia preestablecida como la más cercana, cambia de planeta orbitado
            if (distanceSurface < distanceNearestSurface)
            {
                distanceNearestSurface = distanceSurface;
                gravityNearest = acceleration;
                actualPlanet = planet;
            }
        }

        Vector3 normal = -gravityNearest.normalized;
        transform.rotation = Quaternion.FromToRotation(transform.up, normal) * transform.rotation;
    }

    /// <summary>
    /// Actualiza la posición del jugador con base en la velocidad
    /// </summary>
    private void UpdatePosition()
    {
        position += velocity * Time.fixedDeltaTime;
        transform.position = position;
    }

    /// <summary>
    /// Actualiza el texto de la UI con respecto a la velocidad y al planeta en órbita actual
    /// </summary>
    private void UpdateText()
    {
        textOrbit.text = actualPlanet.name;
        textVelocity.text = velocity.ToString();
    }
}
