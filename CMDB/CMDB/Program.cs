using CMDB.DAO;
using CMDB.DATA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDB
{
    class Program
    {
        static bool exit = false;
        static void Main(string[] args)
        {
            while (!exit)
            {
                Console.WriteLine("1 - Crear Elemento de Configuración");
                Console.WriteLine("2 - Agregar Dependencia");
                Console.WriteLine("3 - Modificar Elemento de Configuración");
                Console.WriteLine("4 - Ver listado de items");
                Console.WriteLine("5 - Salir");
                Console.WriteLine("Favor introducir a continuación el número de la opción deseada seguido de Enter");
                Console.WriteLine();
                string selectedOption = Console.ReadLine();
                if (isValidOption(selectedOption,5))
                {
                    performAction(selectedOption);
                }
                else
                {
                    Console.WriteLine("Opción inválida. Intente de nuevo.");
                }
            }
        }
        static void performAction(string selectedOption)
        {
            switch (selectedOption)
            {
                case "1":
                    addConfigurationItem();
                    break;
                case "2":
                   // addDependency();
                    break;
                case "3":
                    modifyConfigurationItem();
                    break;
                case "4":
                    showFullItemList();
                    break;
                case "5":
                    exit = true;
                    break;

            }
        }

        private static void showFullItemList()
        {
            Console.Clear();
            configurationItemDAO configurationItemDAO = new configurationItemDAO();
            List<configurationItem> allConfigurationItems = configurationItemDAO.getConfigurationItems();

            Console.WriteLine("\n\n Listado de elementos de configuración \n\n");
            foreach (configurationItem ci in allConfigurationItems)
            {
                Console.WriteLine(ci.toString());
                if(ci.dependencies!= null && ci.dependencies.Any())
                {
                    Console.WriteLine("Dependencias: ");
                    foreach (dependencies dependency in ci.dependencies)
                    {
                        Console.WriteLine("        "+dependency.dependsOn.toString());
                    }
                    Console.WriteLine();
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
            Console.WriteLine("Para continuar presione Enter");
            Console.ReadLine();
            Console.Clear();
        }

        private static void modifyConfigurationItem()
        {
            Console.Clear();
            configurationItemDAO configurationItemDAO = new configurationItemDAO();
            bool notDone = true;
            while (notDone)
            {
                listAllConfigurationItems();
                Console.WriteLine("Favor introducir el ID del elemento de configuración a modificar");
                Console.WriteLine("Para volver atrás introduzca q");
                string configurationItemID = Console.ReadLine();
                int ID;
                if (configurationItemID == "q")
                {
                    notDone = false;
                    continue;
                }
                if (!int.TryParse(configurationItemID, out ID))
                {
                    Console.WriteLine("Selección invalida. Favor seleccionar un número correspondiente al ID de un elemento de configuración previamente registrado");
                }
                else
                {
                    if (!configurationItemDAO.existsConfigurationItemWithID(ID))
                    {
                        Console.WriteLine("Este ID no corresponde a un Elemento de configuración previamente registrado, favor intentar nuevamente");
                    }
                    else
                    {
                        bool modified = false;
                        configurationItem configurationItem = configurationItemDAO.getConfigurationItem(ID);
                        while (!modified)
                        {
                            Console.Clear();
                            Console.WriteLine(String.Format("Elemento Seleccionado: {0}", configurationItem.nombre));
                            Console.WriteLine("1 - Actualizar");
                            Console.WriteLine("2 - Deprecar");
                            Console.WriteLine("Favor introducir a continuación el número de la opción deseada seguido de Enter");
                            Console.WriteLine("Para volver atrás introduzca q");
                            Console.WriteLine();
                            string selectedOption = Console.ReadLine();
                            if (selectedOption == "q")
                            {
                                modified = true;
                                continue;
                            }
                            if (isValidOption(selectedOption,2))
                            {
                                switch (selectedOption)
                                {
                                    case "1":
                                        bool actualised = false;
                                        while (!actualised)
                                        {
                                            Console.Clear();
                                            Console.WriteLine("Introduzca la nueva versión:");
                                            Console.WriteLine("Para volver atrás introduzca q");
                                            string newVersion = Console.ReadLine();
                                            if (newVersion == "q")
                                            {
                                                actualised = true;
                                                continue;
                                            }
                                            if (!configurationItemDAO.isValidVersion(newVersion))
                                            {
                                                Console.WriteLine("Esta versión no cumple con el formato requerido: ");
                                                Console.WriteLine("x.x.x");
                                                continue;
                                            }
                                            else
                                            {
                                                if (configurationItemDAO.isMajorVersionChange(configurationItem.version, newVersion))
                                                {
                                                    Console.WriteLine(string.Format("Elementro de configuración {0} no puede ser actualizado a versión {1} \ndebido a que es un cambio de versión mayor y los siguientes elementos de configuración dependen de el:", configurationItem.nombre, newVersion));
                                                    Console.WriteLine();
                                                    Console.WriteLine();
                                                    foreach (dependencies ci in configurationItem.dependees.ToList())
                                                    {
                                                        Console.WriteLine(ci.dependee.toString());
                                                    }
                                                    Console.WriteLine();
                                                }
                                                else
                                                {
                                                    configurationItem.version = newVersion;
                                                    configurationItemDAO.save();
                                                    Console.WriteLine("Elemento de configuración actualizado exitosamente \n\n");
                                                }
                                                actualised = true;
                                                modified = true;
                                                notDone = false;
                                            }
                                        }
                                        break;
                                    case "2":
                                        if (configurationItem.dependees.Any(ci => !(ci.dependee.deprecated ?? false)))
                                        {
                                            Console.WriteLine(string.Format("Elementro de configuración {0} no puede ser deprecado \ndebido a que los siguientes elementos de configuración dependen de el:", configurationItem.nombre));
                                            Console.WriteLine();
                                            Console.WriteLine();
                                            foreach (dependencies ci in configurationItem.dependees.ToList())
                                            {
                                                Console.WriteLine(ci.dependee.toString());
                                            }
                                            Console.WriteLine();
                                        }
                                        else
                                        {
                                            configurationItemDAO.deprecate(configurationItem.configurationItemId);
                                            Console.WriteLine("Elemento de configuración deprecado exitosamente.\n\n");
                                        }
                                        modified = true;
                                        notDone = false;
                                        break;
                                }

                            }
                            else
                            {
                                Console.WriteLine("Opción inválida. Intente de nuevo.");
                            }
                        }

                    }
                }
            }
        }

        static bool isValidOption(string option, int max)
        {
            int dummy = 0;
            int.TryParse(option, out dummy);
            if(dummy!= null && dummy >0 && dummy <= max)
            {
                return true;
            }
            return false;
        }

        static void addConfigurationItem()
        {
            configurationItemDAO configurationItemDAO = new configurationItemDAO();
            string nombre = "";
            string version = "";
            string descripcion = "";
            bool complete = false;
            bool validInput = false;

            while (!complete)
            {
                while (!validInput)
                {
                    Console.Clear();
                    Console.WriteLine("Introduzca el nombre del elemento de configuración: ");
                    Console.WriteLine("Para volver atrás introduzca q");
                    nombre = Console.ReadLine();
                    if (string.IsNullOrEmpty(nombre))
                    {
                        Console.WriteLine("Debe introducir un nombre.");

                    }
                    else
                    {
                        if(nombre == "q")
                        {
                            complete = true;
                            validInput = true;
                            continue;
                        }
                        if (configurationItemDAO.existsConfigurationItemWithSameName(nombre))
                        {
                            Console.WriteLine("Este nombre ya está registrado.");

                        }
                        else
                        {
                            validInput = true;
                        }
                    }
                }
                validInput = false;
                while (!validInput && !complete)
                {
                    Console.WriteLine("Introduzca la versión: ");
                    Console.WriteLine("Para volver atrás introduzca q");
                    version = Console.ReadLine();
                    if (string.IsNullOrEmpty(version))
                    {
                        Console.WriteLine("Debe introducir una versión.");

                    }
                    else
                    {
                        if (version == "q")
                        {
                            complete = true;
                            validInput = true;
                            continue;
                        }
                        if (!configurationItemDAO.isValidVersion(version))
                        {
                            Console.WriteLine("Esta versión no cumple con el formato requerido: ");
                            Console.WriteLine("x.x.x");

                        }
                        else
                        {
                            validInput = true;
                        }
                    }

                }
                validInput = false;
                while (!validInput && !complete)
                {
                    Console.WriteLine("¿Desea agregar una descripción? y/n");
                    Console.WriteLine("Para volver atrás introduzca q");
                    string input = Console.ReadLine();
                    if (input == "q")
                    {
                        validInput = true;
                        complete = true;
                        continue;
                    }
                    if (input.ToLower() != "y" && input.ToLower() != "n")
                    {
                        Console.WriteLine("Favor introducir una opción válida : y/n");

                    }
                    else
                    {
                        switch (input.ToLower())
                        {
                            case "y":
                                Console.WriteLine("Favor introducir una descripción");
                                descripcion = Console.ReadLine();
                                break;
                            case "n":
                                break;
                        }
                        validInput = true;
                    }
                }

                configurationItem configurationItem = new configurationItem();
                configurationItem.nombre = nombre;
                configurationItem.version = version;
                configurationItem.descripcion = descripcion;
                List<configurationItem> itemsActuales = configurationItemDAO.getConfigurationItems();
                if (itemsActuales != null && itemsActuales.Count() > 0 && itemsActuales.Any(ci => !(ci.deprecated ?? false)))
                {
                    validInput = false;
                    while (!validInput && !complete)
                    {
                        Console.WriteLine("¿Desea agregar una dependencia? y/n");
                        Console.WriteLine("Para volver atrás introduzca q");
                        string input = Console.ReadLine();
                        if (input == "q")
                        {
                            validInput = true;
                            complete = true;
                            continue;
                        }
                        if (input.ToLower() != "y" && input.ToLower() != "n")
                        {
                            Console.WriteLine("Favor introducir una opción válida : y/n");

                        }
                        else
                        {
                            switch (input.ToLower())
                            {
                                case "y":
                                    List<int> dependenciesList = getDependencieslist();
                                    configurationItem.dependenciesList = dependenciesList;
                                    break;
                                case "n":
                                    break;
                            }
                            validInput = true;
                        }
                    }

                }

                if (!complete)
                {
                    configurationItemDAO.addConfigurationItem(configurationItem);

                    Console.WriteLine("Se ha creado un elemento de configuracion exitosamente. Presione Enter para continuar.");
                    Console.ReadLine();
                    Console.Clear();
                    complete = true;
                }
            }
        }

        private static List<int> getDependencieslist()
        {
            Console.Clear();
            configurationItemDAO configurationItemDAO = new configurationItemDAO();
            List<int> dependenciesIds = new List<int>();
            bool notDone = true;
            while (notDone)
            {
                bool validInput = false;
                while (!validInput)
                {
                    Console.Clear();
                    listAllConfigurationItems();
                    Console.WriteLine("Favor introducir el ID de la dependencia a agregar");
                    Console.WriteLine("Para volver atrás introduzca q");
                    string dependencyID = Console.ReadLine();
                    int ID;
                    if (dependencyID == "q")
                    {
                        return null;
                    }
                    if (!int.TryParse(dependencyID, out ID))
                    {
                        Console.WriteLine("Selección invalida. Favor seleccionar un número correspondiente al ID de un elemento de configuración previamente registrado");
                    }
                    else
                    {
                        if (!configurationItemDAO.existsConfigurationItemWithID(ID))
                        {
                            Console.WriteLine("Este ID no corresponde a un Elemento de configuración previamente registrado, favor intentar nuevamente");
                        }
                        else
                        {
                            if (dependenciesIds.Contains(ID))
                            {
                                Console.WriteLine("Esta dependencia ya ha sido seleccionada, favor intentar nuevamente");
                            }
                            else
                            {
                                dependenciesIds.Add(ID);
                                validInput = true;
                            }
                        }
                    }
                }
                validInput = false;
                while (!validInput)
                {
                    Console.WriteLine("¿Desea agregar otra dependencia? y/n");
                    Console.WriteLine("Para volver atrás introduzca q");
                    string input = Console.ReadLine();
                    if (input == "q")
                    {
                        return null;
                    }
                    if (input.ToLower() != "y" && input.ToLower() != "n")
                    {
                        Console.WriteLine("Favor introducir una opción válida : y/n");

                    }
                    else
                    {
                        switch (input.ToLower())
                        {
                            case "y":
                                break;
                            case "n":
                                notDone = false;
                                break;
                        }
                        validInput = true;
                    }
                }
            }

            return dependenciesIds;
        }

        private static void listAllConfigurationItems()
        {
            Console.Clear();
            configurationItemDAO configurationItemDAO = new configurationItemDAO();
            List<configurationItem> allConfigurationItems = configurationItemDAO.getConfigurationItems();

            Console.WriteLine("\n\n Listado de elementos de configuración \n\n");
            foreach (configurationItem ci in allConfigurationItems)
            {
                Console.WriteLine(ci.toString());
            }
        }
    }
}
