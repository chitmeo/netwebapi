using System.Runtime.Serialization;

namespace Dev.Common;

public enum DataProvider
{
    [EnumMember(Value = "mariadb")]
    MariaDB,

    [EnumMember(Value = "mysql")]
    MySQL,

    [EnumMember(Value = "sqlserver")]
    SqlServer
}