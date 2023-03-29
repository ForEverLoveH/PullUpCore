using MiniExcelLibs;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace CameraADCoreModel.GameModel
{
    public class ExcelUtils
    {
        public static void MiniExcel_OutPutExcel(string path, object value)
        {
            MiniExcel.SaveAs(path, value);
        }

        public static bool OutPutExcel(List<Dictionary<string, string>> ldic, string path)
        {
            bool result = false;
            if (ldic.Count == 0) return result;

            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet("Sheet1");//创建一个名称为Sheet0的表;
            HSSFRow row = (HSSFRow)sheet.CreateRow(0);//（第一行写标题)
            Dictionary<string, string> dict = ldic[0];
            int coloum = 0;
            foreach (var key in dict.Keys)
            {
                row.CreateCell(coloum).SetCellValue(key);//第一列标题，
                coloum++;
            }
            for (int i = 0; i < ldic.Count; i++)
            {
                row = (HSSFRow)sheet.CreateRow(i + 1);
                coloum = 0;
                dict = ldic[i];
                foreach (var key in dict.Keys)
                {
                    row.CreateCell(coloum).SetCellValue(dict[key]);//第一列标题，
                    coloum++;
                }
            }
            //文件写入的位置
            using (FileStream fs = File.OpenWrite(path))
            {
                workbook.Write(fs);//向打开的这个xls文件中写入数据  
                result = true;
            }
            return result;
        }
    }
}