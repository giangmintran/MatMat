namespace MatMatShop.Utils
{
    public static class DisplayText
    {
        public static string GetTag(string tag)
        {
            tag = tag.ToLower();
            switch (tag)
            {
                case "bomber":
                    return "Bomber";
                case "bong_chay":
                    return "Bóng chày";
                case "khoac_gio":
                    return "Khoác gió";
                case "thun_the_thao":
                    return "Thun thể thao";
                case "pass":
                    return "Pass";
            }
            return string.Empty;
        }
    }
}
