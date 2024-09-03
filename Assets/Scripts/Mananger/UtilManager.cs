using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class UtilManager : Singleton<UtilManager>
{
    public string CreatedHashPwd(string pwd)
    {
        // SHA256 해시 객체 생성
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // 문자열을 바이트 배열로 변환
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(pwd));

            // 바이트 배열을 16진수 문자열로 변환
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
