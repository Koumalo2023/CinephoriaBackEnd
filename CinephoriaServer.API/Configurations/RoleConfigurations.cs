namespace CinephoriaServer.API.Configurations
{
    public static class RoleConfigurations
    {
        public static string Admin => EnumConfig.UserRole.Admin.ToString();
        public static string Employee => EnumConfig.UserRole.Employee.ToString();
        public static string User => EnumConfig.UserRole.User.ToString();
    }
}