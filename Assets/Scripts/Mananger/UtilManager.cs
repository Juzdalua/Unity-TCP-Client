using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class UtilManager : Singleton<UtilManager>
{
    public string CreatedHashPwd(string pwd)
    {
        // SHA256 �ؽ� ��ü ����
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // ���ڿ��� ����Ʈ �迭�� ��ȯ
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(pwd));

            // ����Ʈ �迭�� 16���� ���ڿ��� ��ȯ
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
