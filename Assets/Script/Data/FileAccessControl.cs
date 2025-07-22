using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class FileAccessControl
{
    const byte FOURBIT = 4;
    const byte ONEBYTE = 8;
    const byte TWOBYTE = 16;
    const byte THREEBYTE = 24;

    int CheckNumCaculate(byte[] data)
    {
        int csum = 0;
        for(int i=FOURBIT;i<data.Length;i++)
        {
            csum = data[i];
            csum <<= 1;
        }
        return csum;
    }
    static void EncryptionSystem(byte[] data)
    {
        for (int i=0; i<data.Length;i++)
        {
            data[i] ^= 0xff;
        }
    }
    public bool SaveFileSystem<T>(string folderPath, string fileName, T saveData) where T : struct
    {
        fileName = folderPath + fileName;
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        byte[] binaryData;
        using (MemoryStream ms = new MemoryStream())
        {
            BinaryFormatter writer=new BinaryFormatter();
            writer.Serialize(ms, saveData);
            binaryData = new byte[ms.Length + FOURBIT];
            ms.ToArray().CopyTo(binaryData,FOURBIT);

            int csum =CheckNumCaculate(binaryData);

            binaryData[0]=(byte)((csum>>THREEBYTE) & 0xff);
            binaryData[1]=(byte)((csum>>TWOBYTE) & 0xff);
            binaryData[2]=(byte)((csum>>ONEBYTE) & 0xff);
            binaryData[3]=(byte)(csum & 0xff);
        }
        
        EncryptionSystem(binaryData);

        using(FileStream fs=new FileStream(fileName,FileMode.Create,FileAccess.Write))
        {
            try
            {
                fs.Write(binaryData, 0, binaryData.Length);
            }
            catch(Exception)
            {
                return false;
            }
        }
        return true;
    }

    public bool LoadFileSystem<T>(string folderPath, string fileName, out T loadData) where T : struct
    {
        fileName = folderPath + fileName;
        if (!Directory.Exists(folderPath))
        {
            loadData = default;
            return false;
        }
        if (!File.Exists(fileName))
        {
            loadData = default;
            return false;
        }

        byte[] fileData;
        using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            try
            {
                fileData = new byte[fs.Length];
                fs.Read(fileData, 0, fileData.Length);
            }
            catch (Exception)
            {
                loadData = default;
                return false;
            }
        }
        
        EncryptionSystem(fileData);

        int csum=CheckNumCaculate(fileData);
        int sum = (fileData[0] << THREEBYTE) | (fileData[1] << TWOBYTE) | (fileData[2] << ONEBYTE) | (fileData[3]);
        if(csum!=sum)
        {
            loadData = default;
            return false;
        }

        byte[] binaryData = new byte[fileData.Length - FOURBIT];
        Array.Copy(fileData, FOURBIT, binaryData, 0, binaryData.Length);

        using (MemoryStream ms = new MemoryStream(binaryData))
        {
            BinaryFormatter reader=new BinaryFormatter();
            loadData=(T)reader.Deserialize(ms);
        }
        return true;
    }

    // 設定ファイル読み込み
    public void LoadSetthingData(ref AplSettings setthingData)
    {
        string workPath = Path.Combine(Application.persistentDataPath, "AplSettinhFile.json");
        string json = File.ReadAllText(workPath);
        setthingData=JsonUtility.FromJson<AplSettings>(json);
    }
    // 設定ファイル更新、生成
    public void SaveSetthingData(AplSettings aplSettings)
    {
        string workPath = Path.Combine(Application.persistentDataPath, "AplSettinhFile.json");
        string json = JsonUtility.ToJson(aplSettings, true);
        File.WriteAllText(workPath, json);
    }
}