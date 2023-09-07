namespace Api;

public static class ApiEndpoints
{
    private const string ApiBase = "/api";

    public static class Users
    {
        private const string Base = $"{ApiBase}/users";
        // Auth
        private const string AuthBase = $"{ApiBase}/auth/user";
        public const string Register = $"{AuthBase}/register";

        public static class Posts
        {
            public const string Create = $"{Base}/{{userId}}/posts";
        }
    }
}