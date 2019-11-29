using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace BezorgDirect.BezorgersApplicatie.BusinessLogic.Services
{
    /* This class was setup by Lennart de Waart (563079) */
    public class AuthenticationsService : IAuthenticationsService // AuthenticationsService should contain everything in contract IAuthenticationsService
    {
        private readonly ILogger _logger;
        private readonly string _tokenCheck;

        /// <summary>
        /// Public constructor, unavailable outside this class
        /// </summary>
        /// <param name="logger"></param>
        public AuthenticationsService(ILogger logger)
        {
            this._logger = logger;
            this._tokenCheck = @"\S*(\S*([a-zA-Z]\S*[0-9])|([0-9]\S*[a-zA-Z]))\S*";
        }       

        /* This function was written by Lennart de Waart (563079) */
        /// <summary>
        /// Public method which determines if a given (admin) token is valid.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="isAdminToken"></param>
        /// <returns>true or false</returns>
        public bool IsTokenValid(string token, bool isAdminToken)
        {
            try
            {
                // Regex to check if token contains at least a letter and a number and contains zero spaces
                if (!string.IsNullOrEmpty(token) && Regex.Match(token, _tokenCheck, RegexOptions.IgnoreCase).Success)
                {
                    if (isAdminToken)
                    {
                        dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("../../../../BezorgDirect.BezorgersApplicatie.API/variables.json"));
                        if (token == (string)config["AdminToken"])
                            return true;
                        else throw new Exception($"Token {token} is not a valid admin token.");
                    }
                    else return true;
                }
                else throw new Exception($"Token {token} is not a valid token.");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"IAuthenticationsService says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
                return false;
            }
        }

        public string getTokenFromHeader(string authHeader)
        {
            string token = authHeader.ToString().Replace("Bearer ", "");

            if (IsTokenValid(token, false))
            {
                return token;
            }
            else
            {
                throw new TokenException("Could not get token from header!");
            }
        }
    }

    public class TokenException : Exception
    {
        public TokenException()
        {
        }

        public TokenException(string message)
            : base(message)
        {
        }

        public TokenException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
