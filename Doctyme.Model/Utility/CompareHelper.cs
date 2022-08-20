using System;
using System.Reflection;

namespace Doctyme.Model
{
    public class CompareHelper
    {
        public static void CopyPropertyValues(object source, object destination)
        {
            var destProperties = destination.GetType().GetProperties();

            foreach (var sourceProperty in source.GetType().GetProperties())
            {
                foreach (var destProperty in destProperties)
                {
                    if (destProperty.Name == sourceProperty.Name && destProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                    {
                        destProperty.SetValue(destination, sourceProperty.GetValue(source, new object[] { }), new object[] { });
                        break;
                    }
                }
            }
        }

        public static void SetValue(object inputObject, string propertyName, object propertyVal)
        {
            //find out the type
            Type type = inputObject.GetType();

            //get the property information based on the type
            System.Reflection.PropertyInfo propertyInfo = type.GetProperty(propertyName);

            //find the property type
            Type propertyType = propertyInfo.PropertyType;

            //Convert.ChangeType does not handle conversion to nullable types
            //if the property type is nullable, we need to get the underlying type of the property
            var targetType = IsNullableType(propertyInfo.PropertyType) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;

            //Returns an System.Object with the specified System.Type and whose value is
            //equivalent to the specified object.
            propertyVal = Convert.ChangeType(propertyVal, targetType);

            //Set the value of the property
            propertyInfo.SetValue(inputObject, propertyVal, null);

        }

        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        public static NewHistoryInstance CreateInstance(string historyTable)
        {
            try
            {
                // find bin folder path
                var binPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
                // Load NetworkApp.Model.dll assembly from bin folder
                Assembly NetworkAppModelAssembly = Assembly.LoadFile(binPath + "\\NetworkApp.Model.dll");
                // Make FullName with Namespace and class name
                string fullName = "NetworkApp.Model." + historyTable;

                // Type of Class
                Type historyType = NetworkAppModelAssembly.GetType();

                // Create instance of dynamic class (Unwrap : Returns the wrapped object).
                object historyTableIns = Activator.CreateInstance(NetworkAppModelAssembly.FullName, fullName).Unwrap();

                return new NewHistoryInstance() { HistoryClassInstance = historyTableIns, HistoryType = historyType };
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                throw;
            }
        }
    }
    public class NewHistoryInstance
    {
        public object HistoryClassInstance { get; set; }
        public Type HistoryType { get; set; }
    }
}
