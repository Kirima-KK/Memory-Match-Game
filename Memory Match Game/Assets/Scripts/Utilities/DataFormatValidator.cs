using Newtonsoft.Json;

namespace MemoryMatch.Utilities
{
    public static class DataFormatValidator
    {
        public static bool IsArgumentContain<T>(object[] args, out T value)
        {
            value = default;

            foreach(var arg in args)
            {
                switch(arg.GetType().ToString())
                {
                    case "System.String":
                        if(typeof(T).Name == "String")
                        {
                            value = (T)arg;
                            return true;
                        }

                        break;

                    case "Newtonsoft.Json.Linq.JObject":
                        value = JsonConvert.DeserializeObject<T>(arg.ToString());
                        return true;

                    default:
                        if(arg.GetType() == typeof(T) || arg is T)
                        {
                            value = (T)arg;
                            return true;
                        }

                        break;
                }
            }

            return false;
        }
    }
}