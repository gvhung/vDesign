using Base.Attributes;
using System.ComponentModel.DataAnnotations;

namespace WebUI.Models.ContentWidgets
{
    public class InternalImageVm : InternalFileVm
    {
        [DetailView("�������� (������������ ��������)", TabName = "�������������")]
        public bool Original { get; set; }

        [DetailView("������", TabName = "�������������")]
        public int Width { get; set; }

        [DetailView("������", TabName = "�������������")]
        public int Height { get; set; }

        [DetailView("��������� �����������", Order = 2)]
        [MaxLength(255)]
        public string Title { get; set; }

        [DetailView("�������������� �����", Order = 2)]
        [MaxLength(255)]
        public string Alt { get; set; }

        [DetailView("������� �����������", Order = 2)]
        [MaxLength(255)]
        public string ImageSignature { get; set; }

        public InternalImageVm()
        {
            this.Original = true;
            this.Height = 300;
            this.Width = 300;
        }
    }
}