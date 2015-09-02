using System;
using System.Reflection;
using System.Web.Mvc;

namespace WebUI.Helpers
{
    public class ExportColumn
    {
        public ModelMetadata ModelMetadata { get; set; }

        public string Title { get; set; }

        public Type Type { get; set; }

        public string PropertyName { get; set; }

        public string PropertyFullName { get; set; }
    }

    public static class ExportWriter
    {
        private static Object GetPropValue(this Object obj, String name)
        {
            foreach (String part in name.Split('.'))
            {
                if (obj == null)
                {
                    return null;
                }

                Type type = obj.GetType();

                PropertyInfo info = type.GetProperty(part);

                if (info == null)
                {
                    return null;
                }

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        //private static void PopulateWorkbook(Workbook workbook, IEnumerable collection, GridPreset preset, ViewModelConfig config, IReadOnlyCollection<ViewModelConfig> configs, IFileSystemService fileService)
        //{
        //    Type type = config.EntityType;

        //    List<object> list = collection.Cast<object>().ToList();

        //    List<PropertyInfo> props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.GetCustomAttribute<JsonIgnoreAttribute>() == null).ToList();

        //    List<ExportColumn> columns = new List<ExportColumn>();

        //    foreach (ColumnSetting column in preset.Columns.Where(x => x.Exportable))
        //    {
        //        ExportColumn ecolumn = new ExportColumn();

        //        string name = column.Member;

        //        ecolumn.Type = props.First(x => x.Name == column.Member).PropertyType;

        //        if (ecolumn.Type.IsBaseObject())
        //        {
        //            name = name + "." + configs.Where(x => x.EntityType == ecolumn.Type).First().LookupProperty;
        //        }

        //        ecolumn.PropertyName = column.Member;

        //        ecolumn.PropertyFullName = name;

        //        ecolumn.Title = column.Title;

        //        ecolumn.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, type, column.Member);

        //        columns.Add(ecolumn);
        //    }

        //    #region init workbook

        //    Worksheet sheet = workbook.Worksheets[0];

        //    sheet.Name = config.Title;

        //    Cells cells = sheet.Cells;

        //    int rowIndex = 0;

        //    int firstColumn = 1;

        //    #endregion

        //    #region header

        //    Style headerstyle = cells[0, 0].GetStyle();
        //    Style subCollectionStyle = cells[0, 0].GetStyle();
        //    Style baseStyle = cells[0, 0].GetStyle();

        //    headerstyle.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
        //    headerstyle.Borders[BorderType.TopBorder].Color = Color.Black;
        //    headerstyle.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
        //    headerstyle.Borders[BorderType.BottomBorder].Color = Color.Black;
        //    headerstyle.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
        //    headerstyle.Borders[BorderType.LeftBorder].Color = Color.Black;
        //    headerstyle.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
        //    headerstyle.Borders[BorderType.RightBorder].Color = Color.Black;

        //    headerstyle.Font.IsBold = true;


        //    subCollectionStyle.Font.IsItalic = true;
        //    baseStyle.IsTextWrapped = true;

        //    cells[rowIndex, 0].Value = "№";
        //    cells[rowIndex, 0].SetStyle(headerstyle);

        //    for (int i = 0; i < columns.Count; i++)
        //    {
        //        sheet.AutoFitColumn(i);

        //        cells[rowIndex, i + firstColumn].Value = columns[i].Title;

        //        cells[rowIndex, i + firstColumn].SetStyle(headerstyle);
        //    }

        //    rowIndex++;

        //    #endregion

        //    #region body

        //    int maxNestedCollectionCount = 0;

        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        cells[rowIndex, 0].Value = i + 1;

        //        for (int j = 0; j < columns.Count; j++)
        //        {
        //            if (columns[j].ModelMetadata.DataTypeName == "Image")
        //            {
        //                FileData file = GetPropValue(list[i], columns[j].PropertyName) as FileData;

        //                if (file != null)
        //                {
        //                    string path = fileService.GetTempFilePath(file.FileID);

        //                    if (!File.Exists(path))
        //                    {
        //                        path = fileService.GetFilePath(file.FileID);
        //                    }

        //                    sheet.Cells.Rows[rowIndex].Height = 100;

        //                    if (File.Exists(path))
        //                    {
        //                        int pic = sheet.Pictures.Add(rowIndex, j + firstColumn, rowIndex + 1, j + firstColumn + 1, path);

        //                        sheet.Pictures[pic].IsLockAspectRatio = true;
        //                    }

        //                    continue;
        //                }
        //            }
        //            else if (columns[j].Type.IsBaseCollection())
        //            {
        //                int subCollectionRowIndex = 0;

        //                IEnumerable subCollection = GetPropValue(list[i], columns[j].PropertyName) as IEnumerable;

        //                if (subCollection != null)
        //                {
        //                    Type subType = subCollection.GetType().GetGenericArguments()[0];

        //                    ViewModelConfig subConfig = configs.Where(x => x.EntityType == subType).First();

        //                    foreach (var item in subCollection)
        //                    {
        //                        cells[subCollectionRowIndex + rowIndex, j + firstColumn].Value = GetPropValue(item, subConfig.LookupProperty) ?? "-";

        //                        cells[subCollectionRowIndex + rowIndex, j + firstColumn].SetStyle(subCollectionStyle);

        //                        subCollectionRowIndex++;
        //                    }

        //                }

        //                if (maxNestedCollectionCount < subCollectionRowIndex)
        //                {
        //                    maxNestedCollectionCount = subCollectionRowIndex;
        //                }

        //                continue;
        //            }

        //            cells[rowIndex, j + firstColumn].SetStyle(baseStyle);

        //            cells[rowIndex, j + firstColumn].Value = GetPropValue(list[i], columns[j].PropertyFullName);
        //        }

        //        rowIndex = maxNestedCollectionCount > 0 ? maxNestedCollectionCount + rowIndex : rowIndex + 1;

        //        maxNestedCollectionCount = 0;
        //    }


        //    #endregion

        //    #region footer

        //    sheet.AutoFitColumns();

        //    #endregion

        //}

        //public static string SaveFile(IEnumerable collection, ExportType exportType, GridPreset preset, ViewModelConfig config, IReadOnlyCollection<ViewModelConfig> configs, IFileSystemService fileService)
        //{
        //    string id = Guid.NewGuid().ToString("N");

        //    IPathHelper pathHelper = DependencyResolver.Current.GetObjectService<IPathHelper>();

        //    string filepath = Path.Combine(pathHelper.GetTempDirectory(), id);

        //    Workbook wb = new Workbook();

        //    switch (exportType)
        //    {
        //        case ExportType.XLS:
        //            PopulateWorkbook(wb, collection, preset, config, configs, fileService);
        //            wb.Save(filepath, SaveFormat.Excel97To2003);
        //            break;
        //        case ExportType.DOC:
        //            PopulateWorkbook(wb, collection, preset, config, configs, fileService);
        //            wb.Save(filepath, SaveFormat.ODS);
        //            break;
        //        case ExportType.PDF:
        //            PopulateWorkbook(wb, collection, preset, config, configs, fileService);
        //            wb.Save(filepath, SaveFormat.Pdf);
        //            break;
        //        case ExportType.HTML:
        //            PopulateWorkbook(wb, collection, preset, config, configs, fileService);
        //            wb.Save(filepath, SaveFormat.Html);
        //            break;
        //        case ExportType.SVG:
        //            PopulateWorkbook(wb, collection, preset, config, configs, fileService);
        //            wb.Save(filepath, SaveFormat.SVG);
        //            break;
        //        case ExportType.TIFF:
        //            PopulateWorkbook(wb, collection, preset, config, configs, fileService);
        //            wb.Save(filepath, SaveFormat.TIFF);
        //            break;
        //        case ExportType.TXT:
        //            PopulateWorkbook(wb, collection, preset, config, configs, fileService);
        //            wb.Save(filepath, SaveFormat.TabDelimited);
        //            break;

        //        default:
        //            break;
        //    }

        //    return id;
        //}
    }

    public enum ExportType
    {
        XLS = 0,
        DOC = 1,
        PDF = 3,
        HTML = 4,
        TIFF = 5,
        SVG = 6,
        TXT = 7
    }
}