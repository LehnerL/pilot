using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace pilot
{
    public class Model
    {
        public DateTime _date = new DateTime();
        public int _userCode { get; set; }
        public int _langCode { get; set; }

        public Model(DateTime date, int userCode, int langCode)
        {
            _date = date.ToLocalTime();
            _userCode = userCode;
            _langCode = langCode;
        }

        public Model(DataRow dataRow)
        {
            _date = (DateTime)dataRow["date"];
            _userCode = (int)dataRow["userCode"];
            _langCode = (int)dataRow["langCode"];
        }

        public Model getInfo()
        {
            Model model = new Model(this._date, this._userCode, this._langCode);
            return model;
        }

        public void printInfo()
        {
            Console.WriteLine("Date and Time: " + _date.ToString() + ", User Code: " + _userCode + ", Language Code: " + _langCode);
        }
    }
}