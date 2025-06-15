namespace Shopping.Models
{
    public class Paginate
    {
        public int TotalItems { get; set; } // Tong so du lieu
        public int PageSize { get; set; } // Tong so item/trang
        public int TotalPages { get; set; } //Tong so trang
        public int CurrentPage {  get; set; } // Page hien tai
        public int StartPage {  get; set; } //Trang bat dau
        public int EndPage {  get; set; } // Trang ket thuc
        public Paginate() { }

        public Paginate(int total_items, int page, int page_size = 5)
        {
            int total_page = (int)Math.Ceiling((decimal)total_items/(decimal)page_size);
            int current_page = page;

            int start_page = current_page - 5;
            int end_page = current_page + 4;
            if(start_page <= 0)
            {
                end_page = end_page - (start_page - 1);
                start_page = 1;
            }
            if(end_page > total_page)
            {
                end_page = total_page;
                if (end_page > 10)
                    start_page = end_page - 9;
            }
            TotalItems = total_items;
            TotalPages = total_page;
            CurrentPage = current_page;
            StartPage = start_page;
            EndPage = end_page;
            PageSize = page_size;
        } 
    }
}
