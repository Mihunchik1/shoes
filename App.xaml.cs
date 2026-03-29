using System.Windows;

namespace ZOO_magazin
{
    public partial class App : Application
    {
        public static Entities.shueDEEntities Context { get; } = new Entities.shueDEEntities();
        public static Entities.User CurrentUser = null;
    }
}