using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnvironmentRoadController : MonoBehaviour
{
    [HideInInspector] public float Speed = 1;
    public static EnvironmentRoadController Instance { get; private set; }

    [SerializeField] private GameObject[] forestBiome = null;
    [SerializeField] private GameObject[] desertBiome = null;
    [SerializeField] private Transform[] roads = null;
    [SerializeField] private ParticleSystem wind = null;
    [SerializeField] private float roadLength = 50;
    [SerializeField] private float speedMultiplier = 0.1f;
    [SerializeField] private float windSpeedMultiplier = 0.1f;

    private float endPosition = 0;
    private float range = 0;
    private float pastRoad = 0;
    ParticleSystem.MainModule windEffect;
    private List<GameObject[]> biomes;

    void Awake()
    {
        Instance = this;
        endPosition = roads.Length / 2f * roadLength;
        range = roads.Length * roadLength; 
        windEffect = wind.main;
        biomes = new();
        biomes.Add(forestBiome);
        biomes.Add(desertBiome);
        RandomBiome();
    }

    void Update()
    {
        pastRoad += Time.deltaTime * Speed * speedMultiplier;
        for (int i = 0; i < roads.Length; i++)
        {
            roads[i].localPosition = Vector3.right * ((i * roadLength + pastRoad) % range - endPosition);
        }
        windEffect.simulationSpeed = Speed * windSpeedMultiplier;
    }

    public void Wind()
    {
        wind.Play();
    }

    public void RandomBiome()
    {
        int selectedBiome = Random.Range(0, biomes.Count);
        for (int i = 0; i < biomes.Count; i++)
        {
            for (int j = 0; j < biomes[i].Length; j++)
            {
                biomes[i][j].SetActive(i == selectedBiome);
            }
        }
        windEffect.simulationSpeed = Speed * windSpeedMultiplier;
    }
}
