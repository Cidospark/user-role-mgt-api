using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using UserRoleMgtApi.Models.Dtos;

namespace UserRoleMgtApi.Helpers
{
    public static class Util
    {
        public static List<byte[]> HashGenerator(string password)
        {
            byte[] passwordHash;
            byte[] passwordSalt;

            using (var hash = new HMACSHA512())
            {
                passwordSalt = hash.Key;
                passwordHash = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
            var result = new List<byte[]>();
            result.Add(passwordHash);
            result.Add(passwordSalt);
            return result;
        }

        public static bool ComputeHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hash = new HMACSHA512(passwordSalt))
            {
                var genHash = hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                for(int i = 0; i < genHash.Length; i++)
                {
                    if (genHash[i] != passwordHash[i])
                        return false;
                }
            }
            return true;
        }

        public static ResponseDto<T> BuildResponse<T>(bool status, string message, ModelStateDictionary errs, T data)
        {
            var listOfErrorItems = new List<ErrorItem>();

            if (errs != null)
            {
                foreach (var err in errs)
                {
                    ///err.error.errors
                    var key = err.Key;
                    var errValues = err.Value;
                    var errList = new List<string>();
                    foreach (var errItem in errValues.Errors)
                    {
                        errList.Add(errItem.ErrorMessage);
                        listOfErrorItems.Add(new ErrorItem { Key = key, ErrorMessages = errList });
                    }
                }
            }

            var res = new ResponseDto<T>
            {
                Status = status,
                Message = message,
                Data = data,
                Errors = listOfErrorItems
            };

            return res;
        }
    }
}
