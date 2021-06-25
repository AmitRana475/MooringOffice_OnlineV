using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
//using DataBuildingLayer;
using System.IO;
using System.Reflection;


namespace MooringOfficeweb 
{
    public class ITextEvents : iTextSharp.text.pdf.PdfPageEventHelper
    {
        //private readonly ShipmentContaxt sc;
       


    // This is the contentbyte object of the writer  
    PdfContentByte cb;

        // we will put the final number of pages in a template  
        PdfTemplate headerTemplate, footerTemplate;

        // this is the BaseFont we are going to use for the header / footer  
        BaseFont bf = null;

        // This keeps track of the creation time  
        DateTime PrintTime = DateTime.Now;

        #region Fields  
        private string _header;
        #endregion

        #region Properties  
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }
        #endregion

        public override void OnOpenDocument(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            try
            {
                PrintTime = DateTime.Now;
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                headerTemplate = cb.CreateTemplate(100, 100);
                footerTemplate = cb.CreateTemplate(50, 50);
            }
            catch (DocumentException de)
            {
            }
            catch (System.IO.IOException ioe)
            {
            }
        }

        //public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        //{
        //    base.OnEndPage(writer, document);
        //    iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
        //    iTextSharp.text.Font baseFontBig = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
        //    Phrase p1Header = new Phrase("Sample Header Here", baseFontNormal);

        //    Create PdfTable object
        //    PdfPTable pdfTab = new PdfPTable(3);

        //    We will have to create separate cells to include image logo and 2 separate strings
        //    Row 1
        //    PdfPCell pdfCell1 = new PdfPCell();
        //    PdfPCell pdfCell2 = new PdfPCell(p1Header);
        //    PdfPCell pdfCell3 = new PdfPCell();
        //    String text = "Page " + writer.PageNumber + " of ";

        //    Add paging to header
        //    {
        //        cb.BeginText();
        //        cb.SetFontAndSize(bf, 12);
        //        cb.SetTextMatrix(document.PageSize.GetRight(200), document.PageSize.GetTop(45));
        //        cb.ShowText(text);
        //        cb.EndText();
        //        float len = bf.GetWidthPoint(text, 12);
        //        Adds "12" in Page 1 of 12
        //        cb.AddTemplate(headerTemplate, document.PageSize.GetRight(200) + len, document.PageSize.GetTop(45));
        //    }
        //    Add paging to footer
        //    {
        //        cb.BeginText();
        //        cb.SetFontAndSize(bf, 12);
        //        cb.SetTextMatrix(document.PageSize.GetRight(180), document.PageSize.GetBottom(30));
        //        cb.ShowText(text);
        //        cb.EndText();
        //        float len = bf.GetWidthPoint(text, 12);
        //        cb.AddTemplate(footerTemplate, document.PageSize.GetRight(180) + len, document.PageSize.GetBottom(30));
        //    }

        //    Row 2
        //    PdfPCell pdfCell4 = new PdfPCell(new Phrase("Sub Header Description", baseFontNormal));

        //    Row 3
        //    PdfPCell pdfCell5 = new PdfPCell(new Phrase("Date:" + PrintTime.ToShortDateString(), baseFontBig));
        //    PdfPCell pdfCell6 = new PdfPCell();
        //    PdfPCell pdfCell7 = new PdfPCell(new Phrase("TIME:" + string.Format("{0:t}", DateTime.Now), baseFontBig));

        //    set the alignment of all three cells and set border to 0
        //    pdfCell1.HorizontalAlignment = Element.ALIGN_CENTER;
        //    pdfCell2.HorizontalAlignment = Element.ALIGN_CENTER;
        //    pdfCell3.HorizontalAlignment = Element.ALIGN_CENTER;
        //    pdfCell4.HorizontalAlignment = Element.ALIGN_CENTER;
        //    pdfCell5.HorizontalAlignment = Element.ALIGN_CENTER;
        //    pdfCell6.HorizontalAlignment = Element.ALIGN_CENTER;
        //    pdfCell7.HorizontalAlignment = Element.ALIGN_CENTER;

        //    pdfCell2.VerticalAlignment = Element.ALIGN_BOTTOM;
        //    pdfCell3.VerticalAlignment = Element.ALIGN_MIDDLE;
        //    pdfCell4.VerticalAlignment = Element.ALIGN_TOP;
        //    pdfCell5.VerticalAlignment = Element.ALIGN_MIDDLE;
        //    pdfCell6.VerticalAlignment = Element.ALIGN_MIDDLE;
        //    pdfCell7.VerticalAlignment = Element.ALIGN_MIDDLE;

        //    pdfCell4.Colspan = 3;

        //    pdfCell1.Border = 0;
        //    pdfCell2.Border = 0;
        //    pdfCell3.Border = 0;
        //    pdfCell4.Border = 0;
        //    pdfCell5.Border = 0;
        //    pdfCell6.Border = 0;
        //    pdfCell7.Border = 0;

        //    add all three cells into PdfTable
        //    pdfTab.AddCell(pdfCell1);
        //    pdfTab.AddCell(pdfCell2);
        //    pdfTab.AddCell(pdfCell3);
        //    pdfTab.AddCell(pdfCell4);
        //    pdfTab.AddCell(pdfCell5);
        //    pdfTab.AddCell(pdfCell6);
        //    pdfTab.AddCell(pdfCell7);

        //    pdfTab.TotalWidth = document.PageSize.Width - 80f;
        //    pdfTab.WidthPercentage = 70;
        //    pdfTab.HorizontalAlignment = Element.ALIGN_CENTER;

        //    call WriteSelectedRows of PdfTable. This writes rows from PdfWriter in PdfTable
        //    first param is start row. -1 indicates there is no end row and all the rows to be included to write
        //    Third and fourth param is x and y position to start writing
        //    pdfTab.WriteSelectedRows(0, -1, 40, document.PageSize.Height - 30, writer.DirectContent);
        //    set pdfContent value

        //    Move the pointer and draw line to separate header section from rest of page
        //    cb.MoveTo(40, document.PageSize.Height - 100);
        //    cb.LineTo(document.PageSize.Width - 40, document.PageSize.Height - 100);
        //    cb.Stroke();

        //    Move the pointer and draw line to separate footer section from rest of page
        //    cb.MoveTo(40, document.PageSize.GetBottom(50));
        //    cb.LineTo(document.PageSize.Width - 40, document.PageSize.GetBottom(50));
        //    cb.Stroke();
        //}

        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
           
            base.OnEndPage(writer, document);
            iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 13f, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            iTextSharp.text.Font baseFontBig = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8f, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
            Phrase p1Header = new Phrase("Mooring Management System", baseFontNormal);

           

            //string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            var path = AppDomain.CurrentDomain.BaseDirectory + @"Images\" + "logoS.png";
         





         

            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(path);
            image.SetAbsolutePosition(50, 20);


            image.ScaleAbsolute(100,40);
            image.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
            PdfContentByte cbhead = writer.DirectContent;
            PdfTemplate tp = cbhead.CreateTemplate(300, 95);
            tp.AddImage(image);


            cbhead.AddTemplate(tp,0, 842 - 95);


            // Phrase p1Header = new Phrase(cbhead, baseFontNormal);

            //iTextSharp.text.Image imghead = iTextSharp.text.Image.GetInstance(clsAppConfig.ReportHeaderImage);
            string s = "SECTION - MSLMP" + Environment.NewLine + "REVISION -4444 " +  Environment.NewLine + "Uncontrolled Copy";
            //Create PdfTable object  
            PdfPTable pdfTab = new PdfPTable(3);

            pdfTab.TotalWidth = 600f;
            pdfTab.LockedWidth = true;
            float[] widths = new float[] { 150f, 300f, 150f };
            pdfTab.SetWidths(widths);

            //We will have to create separate cells to include image logo and 2 separate strings  
            //Row 1  
            PdfPCell pdfCell1 = new PdfPCell();
            PdfPCell pdfCell2 = new PdfPCell(p1Header);
            PdfPCell pdfCell3 = new PdfPCell(new Phrase(s, baseFontBig));
            //String text = "SECTION  -  MSLMP";

            //Add paging to header  
            //{
            //    cb.BeginText();
            //    cb.SetFontAndSize(bf, 13);
            //    cb.SetTextMatrix(document.PageSize.GetRight(150), document.PageSize.GetTop(55));
            //    cb.ShowText(text);
            //    cb.EndText();
            //    float len = bf.GetWidthPoint(text, 13);
            //    //Adds "12" in Page 1 of 12  
            //    //cb.AddTemplate(headerTemplate, document.PageSize.GetRight(200) + len, document.PageSize.GetTop(45));
            //    cb.AddTemplate(headerTemplate, document.PageSize.GetRight(200) + len, document.PageSize.GetTop(45));
            //}
            //Add paging to footer  
            //{
            //    cb.BeginText();
            //    cb.SetFontAndSize(bf, 12);
            //    cb.SetTextMatrix(document.PageSize.GetRight(180), document.PageSize.GetBottom(30));
            //    cb.ShowText(text);
            //    cb.EndText();
            //    float len = bf.GetWidthPoint(text, 12);
            //    cb.AddTemplate(footerTemplate, document.PageSize.GetRight(180) + len, document.PageSize.GetBottom(30));
            //}

           

            //Row 2  
            PdfPCell pdfCell4;

            pdfCell4 = new PdfPCell(new Phrase("Management Plan (MSMP)", baseFontNormal));
            //pdfCell4.Top = 450;

            //pdfCell4.Top = 50;

            //Row 3   
            PdfPCell pdfCell5 = new PdfPCell();
            PdfPCell pdfCell6 = new PdfPCell();
            PdfPCell pdfCell7 = new PdfPCell();



            //set the alignment of all three cells and set border to 0  
            pdfCell1.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfCell2.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfCell3.HorizontalAlignment = Element.ALIGN_BOTTOM;
            pdfCell4.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfCell5.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfCell6.HorizontalAlignment = Element.ALIGN_CENTER;
            pdfCell7.HorizontalAlignment = Element.ALIGN_CENTER;

            pdfCell3.Top = 20;

            pdfCell2.VerticalAlignment = Element.ALIGN_BOTTOM;
            pdfCell3.VerticalAlignment = Element.ALIGN_BOTTOM;
            pdfCell4.VerticalAlignment = Element.ALIGN_TOP;
            pdfCell5.VerticalAlignment = Element.ALIGN_MIDDLE;
            pdfCell6.VerticalAlignment = Element.ALIGN_MIDDLE;
            pdfCell7.VerticalAlignment = Element.ALIGN_MIDDLE;

            pdfCell3.PaddingTop = 10;

            pdfCell4.Colspan = 3;

            pdfCell1.Border = 0;
            pdfCell2.Border = 0;
            pdfCell3.Border = 0;
            pdfCell4.Border = 0;
            pdfCell5.Border = 0;
            pdfCell6.Border = 0;
            pdfCell7.Border = 0;

            //add all three cells into PdfTable  
            pdfTab.AddCell(pdfCell1);
            pdfTab.AddCell(pdfCell2);
            pdfTab.AddCell(pdfCell3);
            pdfTab.AddCell(pdfCell4);
            pdfTab.AddCell(pdfCell5);
            pdfTab.AddCell(pdfCell6);
            pdfTab.AddCell(pdfCell7);

            pdfTab.TotalWidth = document.PageSize.Width - 80f;
            pdfTab.WidthPercentage = 90;
            //pdfTab.HorizontalAlignment = Element.ALIGN_CENTER;      

            //call WriteSelectedRows of PdfTable. This writes rows from PdfWriter in PdfTable  
            //first param is start row. -1 indicates there is no end row and all the rows to be included to write  
            //Third and fourth param is x and y position to start writing  
            pdfTab.WriteSelectedRows(0, -1, 40, document.PageSize.Height - 30, writer.DirectContent);
            //set pdfContent value  

            //Move the pointer and draw line to separate header section from rest of page  
            cb.MoveTo(20, document.PageSize.Height - 100);
            cb.LineTo(document.PageSize.Width - 20, document.PageSize.Height - 100);
            cb.Stroke();

            //Move the pointer and draw line to separate footer section from rest of page  
            //cb.MoveTo(40, document.PageSize.GetBottom(50));
            //cb.LineTo(document.PageSize.Width - 40, document.PageSize.GetBottom(50));
            cb.Stroke();
        }

        public override void OnCloseDocument(PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnCloseDocument(writer, document);

            headerTemplate.BeginText();
            headerTemplate.SetFontAndSize(bf, 12);
            headerTemplate.SetTextMatrix(0, 0);
            headerTemplate.ShowText((writer.PageNumber - 1).ToString());
            headerTemplate.EndText();

            footerTemplate.BeginText();
            footerTemplate.SetFontAndSize(bf, 12);
            footerTemplate.SetTextMatrix(0, 0);
            footerTemplate.ShowText((writer.PageNumber - 1).ToString());
            footerTemplate.EndText();
        }
    }
}

