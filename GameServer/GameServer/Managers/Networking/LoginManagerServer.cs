﻿using BCrypt.Net;
using MySql.Data.MySqlClient;
using Server.Managers;
using Server.Types;
using System;
using System.Dynamic;

namespace Client.Managers
{
    class LoginManagerServer : LoginManagerHead
    {

        private CharacterPlayer AccountCharacter;

        public LoginManagerServer(string username, string password) : base(username, password)
        {}

        /** True if logged in and  **/
        public bool SetupLogin()
        {
            string[] st = SQLManager.GetDataFromSQL(username);
            string hashed = st[1];
            bool testP = CryptoManager.CheckHash(password, hashed);
            if (testP)
            {
                AccountCharacter = Newtonsoft.Json.JsonConvert.DeserializeObject<CharacterPlayer>(st[0]);
                return true;
            }
            return false;
        }

        public void SetCharacter(CharacterPlayer character)
        {
            AccountCharacter = character;
        }

        public CharacterPlayer GetCharacter()
        {
            return AccountCharacter;
        }

        public override int GetHashCode()
        {
            return GetUniqueID().GetHashCode();
        }
    }
}
