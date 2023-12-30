using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Dropa.Extensions
{
    public static class WebExtensions
    {
        public static string GetIdClaim(this System.Security.Claims.ClaimsPrincipal _user)
        {
            var id = _user.Claims.FirstOrDefault(a => a.Type == "ID")?.Value;
            try
            {
                if (id == null)
                {
                    id = _user.Claims.FirstOrDefault(a => a.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                }
                if (id == null)
                {
                    var _id = _user.Claims.ToList()[0].Value;
                    if (_id != null) id = _id;
                }
            }
            catch
            {
            }
            return id;
        }

        public static string GetEmailClaim(this System.Security.Claims.ClaimsPrincipal _user)
        {
            return _user.Claims.FirstOrDefault(a => a.Type == "Email")?.Value;
        }

        /// <summary>
        /// tr | en gibi kısa dil bilgisi gönderir.
        /// </summary>
        /// <returns></returns>
        public static string GetLocale()
        {
            return Thread.CurrentThread.CurrentCulture.Parent.Name;
        }

        public static string HtmlToText(this string HTMLText, bool HtmlDecode = true)
        {
            if (HTMLText == null)
            {
                return "";
            }
            Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
            var stripped = reg.Replace(HTMLText, "");
            return HtmlDecode ? HttpUtility.HtmlDecode(stripped) : stripped;
        }

        public static string ToErrorMessage(this string str)
        {
            return "<div class='alert alert-danger' style='background-color: #e70c1e52!important;border-color: #e70c1e!important;color: #e70c1e;!important'><b>Hata : </b>" + str + "</div>";
        }

        public static string ToSuccessMessage(this string str)
        {
            return "<div class='alert alert-success' style='background-color: #dff0d8 !important;border-color: #00b63a!important;color: #00b63a!important;'><i class='fa fa-check'></i> <b>Başarılı: </b> " + str + "</div>";
        }

        public static string ToInfoMessage(this string str)
        {
            return "<div class='alert alert-info'><b>Not : </b>" + str + "</div>";
        }

        public static string ToTouchspin(this decimal f)
        {
            return f.ToString().Replace(",", ".");
        }

        public static string ToTouchspin(this float f)
        {
            return f.ToString().Replace(",", ".");
        }

        public static string ToTouchspin(this double f)
        {
            return f.ToString().Replace(",", ".");
        }

        public static string ToStringJoin<T>(this IEnumerable<T> list, string str)
        {
            return string.Join(str, list);
        }

        public static string ToSubString(this string str, int Length, int StartIndex = 0, bool Bla = true)
        {
            if (str == null)
            {
                return null;
            }
            if (str.Length < Length)
            {
                return str;
            }
            else
            {
                if (Bla)
                    return str.Substring(StartIndex, Length) + "...";
                else
                    return str.Substring(StartIndex, Length);
            }
        }

        public static string FriendlyURLTitle(string pTitle)
        {
            if (string.IsNullOrEmpty(pTitle)) return "";
            pTitle = pTitle.Replace(" ", "-");
            pTitle = pTitle.Replace(".", "-");
            pTitle = pTitle.Replace("ı", "i");
            pTitle = pTitle.Replace("İ", "I");
            pTitle = string.Join("", pTitle.Normalize(NormalizationForm.FormD).Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));

            pTitle = HttpUtility.UrlEncode(pTitle);
            string son = Regex.Replace(pTitle, @"\%[0-9A-Fa-f]{2}", "");
            return son.ToSefLink();
        }

        public static string StringReplaceForTR(this string text)
        {
            text = text.Replace("İ", "I");
            text = text.Replace("I", "i");
            text = text.Replace("ı", "i");
            text = text.Replace("Ğ", "G");
            text = text.Replace("ğ", "g");
            text = text.Replace("Ö", "O");
            text = text.Replace("ö", "o");
            text = text.Replace("Ü", "U");
            text = text.Replace("ü", "u");
            text = text.Replace("Ş", "S");
            text = text.Replace("ş", "s");
            text = text.Replace("Ç", "C");
            text = text.Replace("ç", "c");
            text = text.Replace(" ", "_");
            return text;
        }

        public static string TimeAgo(this DateTime? date)
        {
            if (date == null)
            {
                return string.Empty;
            }
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.Now.Ticks - date.Value.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "az önce" : ts.Seconds + " saniye önce";

            if (delta < 2 * MINUTE)
                return "1 dakika önce";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " dakika önce";

            if (delta < 90 * MINUTE)
                return "1 saat önce";

            if (delta < 24 * HOUR)
                return ts.Hours + " saat önce";

            if (delta < 48 * HOUR)
                return "dün";

            if (delta < 30 * DAY)
                return ts.Days + " gün önce";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "1 ay önce" : months + " ay önce";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "1 yıl önce" : years + " yıl önce";
            }
        }

        public static int ToInt(this object data)
        {
            if (data == null)
            {
                return default;
            }

            return (int)Convert.ChangeType(data, typeof(int));
        }

        public static byte ToByte(this object data)
        {
            if (data == null)
            {
                return default;
            }

            return (byte)Convert.ChangeType(data, typeof(byte));
        }

        public static Dictionary<int, string> EnumToDictionary<TEnum>() where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("Generic tip enum değil");
            }

            return Enum.GetValues(typeof(TEnum)).Cast<int>().ToDictionary(e => e, e => Enum.GetName(typeof(TEnum), e).Replace("_0_", "-").Replace("_", " "));
        }

        public static object ChangeType(this object value, Type type)
        {
            try
            {
                if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
                if (value == null) return null;
                if (type == value.GetType()) return value;
                if (type.IsEnum)
                {
                    if (value is string)
                        return Enum.Parse(type, value as string);
                    else
                        return Enum.ToObject(type, value);
                }
                if (!type.IsInterface && type.IsGenericType)
                {
                    Type innerType = type.GetGenericArguments()[0];
                    object innerValue = value.ChangeType(innerType);
                    return Activator.CreateInstance(type, new object[] { innerValue });
                }
                if (value is string && type == typeof(Guid)) return new Guid(value as string);
                if (value is string && type == typeof(Version)) return new Version(value as string);
                if (!(value is IConvertible)) return value;
                return Convert.ChangeType(value, type);
            }
            catch (FormatException)
            {
                return Activator.CreateInstance(type);
            }
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static bool SearchFilter(this string q)
        {
            bool permission = false;
            if (string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q) || q.Length == 1)
            {
                permission = true;
                return permission;
            }
            if (q.Contains("-c-") || q.Contains("-p-"))
            {
                return permission;
            }
            var arr = q.ToCharArray();
            for (int i = 0; i < arr.Length; i++)
            {
                if (!char.IsLetterOrDigit(q[i]))
                {
                    if (q[i].ToString() == " ")
                    {
                        continue;
                    }
                    permission = true;
                    break;
                }
            }
            return permission;
        }

        /// <summary>
        /// Gönderdiğiniz eposa adresinin host kısmını siler.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string ReplaceEmailExt(this string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return "";
            }
            return Regex.Replace(email, @"(@[a-zA-Z0-9._-]+\.[a-zA-Z0-9_-]+)", "");
        }

        public static bool IsValidEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            string emailRegex =
                @"^(?=.{1,256})(?=.{1,64}@.{1,255}$)(?=\S)(?:(?!@)[\w&'*+._%-]+(?:(?:(?<!\\)\.|\b)\w{1,2})*)@(?=\S)(?:(?!-)[A-Za-z0-9-]{1,63}(?:(?:(?<!\\)\.)[A-Za-z]{2,})*)$";
            Regex regex = new Regex(emailRegex);
            return regex.IsMatch(email);
        }

        private static string ToSefLink(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            input = input.ToLower(new CultureInfo("tr-TR"));
            input = SefStage1(input);
            input = SefStage2(input);
            input = Regex.Replace(input, "&.+?;", "");
            input = Regex.Replace(input, "[^.a-z0-9 _-]", "");
            input = Regex.Replace(input, @"\.|\s+", "-");
            input = Regex.Replace(input, "-+", "-");
            input = input.Trim('-');
            return input;
        }

        private static string SefStage1(string input)
        {
            char[] array = new char[input.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < input.Length; i++)
            {
                char let = input[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }

                if (let == '>')
                {
                    inside = false;
                    continue;
                }

                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }

            return new string(array, 0, arrayIndex);
        }

        public static string GetLang()
        {
            return Thread.CurrentThread.CurrentCulture.Name.ToString();
        }

        private static string SefStage2(string input)
        {
            string normalized = input.Replace('ı', 'i').Normalize(NormalizationForm.FormKD);
            char[] array = new char[input.Length];
            int arrayIndex = 0;
            foreach (char c in normalized)
            {
                if (char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    array[arrayIndex] = c;
                    arrayIndex++;
                }
            }

            return new string(array, 0, arrayIndex);
        }
    }
}
