using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using System.Security.Cryptography;


public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }


    private string filePlayerSave = "/PlayerData.dat";
    
    public PlayerSave playerSave { get; private set; }

    public GameObject threeDots;
    public Text ScoreText;
    PlayerData currentGameData;
    private int health;
    public AudioSource cupcakeAudio;


    public int Health {
        get {
            return health;
        }
        set {
            health = value;
            for (int i = 0; i < 3; i++)
            {
                UIFunctionalities.Instance.healths[i].gameObject.SetActive(i < health);
            }
            if (health <= 0)
            {
                UIFunctionalities.Instance.GameOver();
            }
        }
    }
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            playerSave = new PlayerSave();
            Load();
            Health = 0;
        }
    }

    private void OnDestroy()
    {
        Save();
    }

    public void CupcakeCatched(float yPosOfCupcake)
    {
        float scoreDistance = 2f;
        float distance = yPosOfCupcake - threeDots.transform.position.y;
        distance = Mathf.Clamp(distance, -scoreDistance, scoreDistance);
        distance = Mathf.Abs(distance);
        distance = scoreDistance - distance;
        currentGameData.Score += (int)(distance / (scoreDistance / 10f));

        ScoreText.text = currentGameData.Score.ToString();

        cupcakeAudio.Play();
    }

    public void CupcakeMissed()
    {
        currentGameData.Score += -10;

        currentGameData.Score = Mathf.Clamp(currentGameData.Score, 0, currentGameData.Score);

        ScoreText.text = currentGameData.Score.ToString();

        Health--;
    }

    public void ResetScore()
    {
        if (currentGameData.Score != 0)
        {
            // find position for the score
            int i;
            for (i = 0; i < playerSave.HighScores.Count; i++)
            {
                if (playerSave.HighScores[i].Score < currentGameData.Score) break;
            }
            // insert the score
            playerSave.HighScores.Insert(i, currentGameData);
        }

        // Reset current score now
        currentGameData.Score = 0;
        ScoreText.text = currentGameData.Score.ToString();

        Health = 3;

        Save();
    }

    public bool CreateNewUser(string username, string password)
    {
        if (username == null || password == null || username.Length < 4 || password.Length < 4)
        {
            return false;
        }

        // Create hash
        PasswordHash hash = new PasswordHash(username + password);
        byte[] hashBytes = hash.ToArray();

        // Add new user to list
        playerSave.Users.Add(hashBytes);

        // Save the file back. Normally, this is not needed as we save when game is being closed,
        // but a user creation is an important event so we save here as well.
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + filePlayerSave);

            bf.Serialize(file, playerSave);

            file.Close();
        }

        return true;
    }

    public bool Login(string username, string password)
    {        
        // Check if user exists
        foreach (byte[] a in playerSave.Users)
        {
            PasswordHash hash = new PasswordHash(a);
            if (hash.Verify(username + password) == true)
            {   // Found the user. Login SUCCESS
                currentGameData.Username = username;
                currentGameData.Score = 0;

                return true;
            }
        }

        return false;
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + filePlayerSave);
        
        bf.Serialize(file, playerSave);

        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + filePlayerSave))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + filePlayerSave, FileMode.Open);

            playerSave = (PlayerSave)bf.Deserialize(file);

            file.Close();
        }
    }
}

[Serializable]
public struct PlayerData
{
    public string Username;
    public int Score;
}

[Serializable]
public class PlayerSave
{
    public List<byte[]> Users = new List<byte[]>();
    public List<PlayerData> HighScores = new List<PlayerData>();
}

public sealed class PasswordHash
{
    const int SaltSize = 16, HashSize = 20, HashIter = 10000;
    readonly byte[] _salt, _hash;

    public PasswordHash(string password)
    {
        new RNGCryptoServiceProvider().GetBytes(_salt = new byte[SaltSize]);
        _hash = new Rfc2898DeriveBytes(password, _salt, HashIter).GetBytes(HashSize);
    }

    public PasswordHash(byte[] hashBytes)
    {
        Array.Copy(hashBytes, 0, _salt = new byte[SaltSize], 0, SaltSize);
        Array.Copy(hashBytes, SaltSize, _hash = new byte[HashSize], 0, HashSize);
    }

    public PasswordHash(byte[] salt, byte[] hash)
    {
        Array.Copy(salt, 0, _salt = new byte[SaltSize], 0, SaltSize);
        Array.Copy(hash, 0, _hash = new byte[HashSize], 0, HashSize);
    }

    public byte[] ToArray()
    {
        byte[] hashBytes = new byte[SaltSize + HashSize];
        Array.Copy(_salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(_hash, 0, hashBytes, SaltSize, HashSize);
        return hashBytes;
    }

    public byte[] Salt { get { return (byte[])_salt.Clone(); } }

    public byte[] Hash { get { return (byte[])_hash.Clone(); } }

    public bool Verify(string password)
    {
        byte[] test = new Rfc2898DeriveBytes(password, _salt, HashIter).GetBytes(HashSize);
        for (int i = 0; i < HashSize; i++)
            if (test[i] != _hash[i])
                return false;
        return true;
    }
}