using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    [SerializeField] Chamber[] Chambers;
    [SerializeField] TMP_Dropdown ChamberDropdown;
    [SerializeField] TMP_Dropdown ParticleDropdown;
    [SerializeField] Slider LaunchVelSlider;
    [SerializeField] Slider LaunchPitchSlider;
    [SerializeField] Slider LaunchYawSlider;
    [SerializeField] Slider DampingSlider;
    [SerializeField] Slider MassSlider;
    [SerializeField] Slider RestitutionSlider;
    [SerializeField] TMP_Text LaunchVelValue;
    [SerializeField] TMP_Text LaunchPitchValue;
    [SerializeField] TMP_Text LaunchYawValue;
    [SerializeField] TMP_Text DampingValue;
    [SerializeField] TMP_Text MassValue;
    [SerializeField] TMP_Text RestitutionValue;
    [SerializeField] TMP_InputField GravityInput;
    [SerializeField] TMP_InputField FrictionInput;
    [SerializeField] GameObject ChamberSettings;
    [SerializeField] GameObject ParticleSettings;

    GameObject[] ChamberParticles;
    Particle ActiveParticle;
    Chamber ActiveChamber;

    private void Start()
    {
        LoadChamber();
    }

    private void Update()
    {
        if (!isActiveAndEnabled)
        {
            LoadChamber();
        }
    }

    public void LoadChamber()
    {
        ActiveChamber = Chambers[ChamberDropdown.value];
        ChamberParticles = ActiveChamber.Particles;
        ChamberSettings.SetActive(true);
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        foreach (GameObject Particle in ChamberParticles)
        {
            TMP_Dropdown.OptionData optionData = new TMP_Dropdown.OptionData();
            optionData.text = Particle.name;
            options.Add(optionData);
        }
        ParticleDropdown.options = options;
        LoadParticle();
    }

    public void LoadParticle()
    {
        ParticleSettings.SetActive(true);

        ActiveParticle = ChamberParticles[ParticleDropdown.value].GetComponent<Particle>();
        LaunchVelSlider.value = ActiveParticle.LaunchVel;
        LaunchPitchSlider.value = ActiveParticle.LaunchPitch;
        LaunchYawSlider.value = ActiveParticle.LaunchYaw;
        DampingSlider.value = ActiveParticle.DampingFactor;
        MassSlider.value = ActiveParticle.Mass;
        RestitutionSlider.value = ActiveParticle.Restitution;

        LaunchVelValue.text = ActiveParticle.LaunchVel.ToString();
        LaunchPitchValue.text = ActiveParticle.LaunchPitch.ToString();
        LaunchYawValue.text = ActiveParticle.LaunchYaw.ToString();
        DampingValue.text = ActiveParticle.DampingFactor.ToString();
        MassValue.text = ActiveParticle.Mass.ToString();
        RestitutionValue.text = ActiveParticle.Restitution.ToString();
    }

    public void ApplyParticle()
    {
        ActiveParticle.LaunchVel = LaunchVelSlider.value;
        ActiveParticle.LaunchPitch = LaunchPitchSlider.value;
        ActiveParticle.LaunchYaw = LaunchYawSlider.value;        
        ActiveParticle.DampingFactor = DampingSlider.value;
        ActiveParticle.Mass = MassSlider.value;
        ActiveParticle.Restitution = RestitutionSlider.value;
    }

    public void UpdateSlider()
    {
        LaunchVelValue.text = (Mathf.Round(LaunchVelSlider.value * 10.0f) * 0.1f).ToString();
        LaunchPitchValue.text = LaunchPitchSlider.value.ToString();
        LaunchYawValue.text = LaunchYawSlider.value.ToString();
        DampingValue.text = (Mathf.Round(DampingSlider.value * 10.0f) * 0.1f).ToString();
        MassValue.text = MassSlider.value.ToString();
        RestitutionValue.text = (Mathf.Round(RestitutionSlider.value * 10.0f) * 0.1f).ToString();
    }

    public void ApplyChamber()
    {
        ActiveChamber.GravityMag = float.Parse(GravityInput.text);
        ActiveChamber.FrictionMag = float.Parse(FrictionInput.text);
    }
}
