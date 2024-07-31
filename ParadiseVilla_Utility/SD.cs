﻿namespace ParadiseVilla_Utility
{
    public static class SD
    {
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
        public static string SessionToken = "JWTToken";
        public static string CurrentAPIVersion = "v2";

        public const string Role_Admin = "admin";
        public const string Role_Customer = "customer";

        public enum ContentType
        {
            JSON,
            MultipartFormData
        }
    }
}
