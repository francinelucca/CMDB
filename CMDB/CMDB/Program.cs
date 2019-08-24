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
                Console.WriteLine("4 - Generar Reportería");
                Console.WriteLine("5 - Salir");
                Console.WriteLine("Favor introducir a continuación el número de la opción deseada seguido de Enter");
                Console.WriteLine();
                string selectedOption = Console.ReadLine();
                if (isValidOption(selectedOption))
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
                  //  modifyConfigurationItem();
                    break;
                case "4":
                  //  generateReport();
                    break;
                case "5":
                    exit = true;
                    break;

            }
        }
        static bool isValidOption(string option)
        {
            int dummy = 0;
            return int.TryParse(option, out dummy);
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
                if (itemsActuales != null && itemsActuales.Count() > 0)
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
                    complete = true;
                }
            }
        }

        private static List<int> getDependencieslist()
        {
            configurationItemDAO configurationItemDAO = new configurationItemDAO();
            List<int> dependenciesIds = new List<int>();
            listAllConfigurationItems();
            bool notDone = true;
            while (notDone)
            {
                bool validInput = false;
                while (!validInput)
                {
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
