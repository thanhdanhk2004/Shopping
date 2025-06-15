using Newtonsoft.Json;

namespace Shopping.Reponitory
{
    public static class SessionExtensions
    {
        //Luu mot doi tuong vao session nhung truoc khi luu phai chuyen doi thanh json
        public static void set_json(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        //Ham nay chuyen doi nguoi lai cac json thanh cac doi tuong ban dau
        public static T get_Json<T>(this ISession session, string key)
        {
            var session_data = session.GetString(key);
            return session_data == null ? default(T) : JsonConvert.DeserializeObject<T>(session_data);
        }
    }
}
