using CMDB.DATA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDB.DAO
{
    class configurationItemDAO
    {
        CMDB.DATA.CMDBEntities databaseConnection;

        public configurationItemDAO()
        {
            databaseConnection = new DATA.CMDBEntities();
        }

        public List<configurationItem> getConfigurationItems()
        {
            return databaseConnection.configurationItem.ToList();
        }

        public void addConfigurationItem(configurationItem configurationItem)
        {
            databaseConnection.configurationItem.Add(configurationItem);
            if(configurationItem.dependenciesList != null && configurationItem.dependenciesList.Any())
            {
                foreach (var dependency in configurationItem.dependenciesList)
                {
                    dependencies dependencies = new dependencies();
                    dependencies.dependeeId = configurationItem.configurationItemId;
                    dependencies.dependsOnId = dependency;
                    databaseConnection.dependencies.Add(dependencies);
                }
            }
            databaseConnection.SaveChanges();
        }

        public bool existsConfigurationItemWithSameName(String name)
        {
            if(databaseConnection.configurationItem.Any(ci=> ci.nombre == name))
            return true;

            return false;
        }
        
        public void addDependency(int dependee, int dependsOn)
        {
            dependencies dependency = new dependencies();
            dependency.dependeeId = dependee;
            dependency.dependsOnId = dependsOn;
            databaseConnection.dependencies.Add(dependency);
            databaseConnection.SaveChanges();
        }

        public List<dependencies> getAllDependencies(int configurationItemId)
        {
            configurationItem ci = databaseConnection.configurationItem.FirstOrDefault(citem => citem.configurationItemId == configurationItemId);

            if(ci != null)
            {
                return ci.dependencies.ToList();
            }
            return null;
        }

        public bool isValidVersion(string version)
        {
            string[] splitted = version.Split('.');
            if(splitted.Count() != 3)
            {
                return false;
            }
            int dummy;
            if(splitted.Any(c => !int.TryParse(c, out dummy)))
            {
                return false;
            }
            return true;
        }

        public bool existsConfigurationItemWithID(int iD)
        {
            if(this.databaseConnection.configurationItem.Any(ci => ci.configurationItemId == iD))
            {
                return true;
            }
            return false;
        }

        public configurationItem getConfigurationItem(int iD)
        {
            return this.databaseConnection.configurationItem.FirstOrDefault(ci => ci.configurationItemId == iD);
        }

        public void deprecate(int configurationItemId)
        {
            this.databaseConnection.configurationItem.Where(ci => ci.configurationItemId == configurationItemId).First().deprecated = true;
            this.databaseConnection.SaveChanges();
        }

        internal void save()
        {
            this.databaseConnection.SaveChanges();
        }

        internal bool isMajorVersionChange(string version, string newVersion)
        {
            int majorVersionNumber;
            int majorNewVersionNumber;
            int.TryParse(version.Split('.')[0], out majorVersionNumber);
            int.TryParse(newVersion.Split('.')[0], out majorNewVersionNumber);

            if(majorNewVersionNumber != majorVersionNumber)
            {
                return true;
            }
            return false;
        }
    }
}
