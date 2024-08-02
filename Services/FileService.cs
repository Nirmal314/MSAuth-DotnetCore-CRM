using static System.Net.Mime.MediaTypeNames;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;
using MSAuth.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Xrm.Sdk;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Query;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using MSAuth.Interfaces;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfDocument = PdfSharpCore.Pdf.PdfDocument;
using PdfSharpCore.Drawing.Layout;
namespace MSAuth.Services;

public class FileService(ServiceClient service) : IFileService
{
    private readonly ServiceClient _service = service;

    public byte[] PopulateWordTemplate(string templatePath, Contact contact)
    {
        using (MemoryStream memStream = new())
        {
            using (FileStream fileStream = new(templatePath, FileMode.Open, FileAccess.Read))
            {
                fileStream.CopyTo(memStream);
            }

            using (WordprocessingDocument doc = WordprocessingDocument.Open(memStream, true))
            {
                MainDocumentPart mainPart = doc.MainDocumentPart!;

                foreach (SdtElement sdt in mainPart.Document.Descendants<SdtElement>())
                {
                    SdtAlias alias = sdt.Descendants<SdtAlias>().FirstOrDefault()!;

                    if (alias != null)
                    {
                        string sdtTitle = alias.Val?.Value!;

                        switch (sdtTitle.Trim())
                        {
                            case "Full Name":
                                SetSdtElementText(sdt, contact.FullName);
                                break;
                            case "Email":
                                SetSdtElementText(sdt, contact.EMailAddress1);
                                break;
                            case "Position":
                                SetSdtElementText(sdt, contact.hiring_Position.ToString() ?? "");
                                break;
                            case "Department":
                                SetSdtElementText(sdt, contact.hiring_Department.ToString() ?? "");
                                break;
                            case "Hire date":
                                SetSdtElementText(sdt, contact.hiring_HireDate.ToString() ?? "");
                                break;
                            case "Manager":
                                EntityReference managerReference = contact.hiring_Manager;

                                if (managerReference != null)
                                {
                                    Entity manager = _service.Retrieve(managerReference.LogicalName, managerReference.Id, new ColumnSet("fullname"));
                                    SetSdtElementText(sdt, manager.GetAttributeValue<string>("fullname") ?? "");
                                }
                                else
                                    SetSdtElementText(sdt, "");

                                break;
                        }
                    }
                }

                mainPart.Document.Save();
            }

            return memStream.ToArray();
        }
    }

    public byte[] GenerateTasksPdf(List<CrmTask> tasks)
    {
        using var ms = new MemoryStream();
        var document = new PdfDocument();
        var page = document.AddPage();
        var graphics = XGraphics.FromPdfPage(page);
        var font = new XFont("Verdana", 12, XFontStyle.Regular);

        double margin = 20;
        double cellHeight = 80;
        double headerCellHeight = 20;
        double pageWidth = page.Width - 2 * margin;
        double taskNameWidth = pageWidth * 0.4;
        double descriptionWidth = pageWidth * 0.6;

        double yPoint = margin + 20;

        // ? Header
        DrawTableCell(graphics, font, "Task Name", margin, yPoint, taskNameWidth, headerCellHeight, isHeader: true);
        DrawTableCell(graphics, font, "Description", margin + taskNameWidth, yPoint, descriptionWidth, headerCellHeight, isHeader: true);
        yPoint += headerCellHeight;

        // ? Content
        bool isStriped = false;
        foreach (var task in tasks)
        {
            // ? Row background
            DrawTableRowBackground(graphics, isStriped, margin, yPoint, pageWidth, cellHeight);

            // ? Alternate
            isStriped = !isStriped;

            // ? Draw cells 
            DrawTableCell(graphics, font, "  " + task.hiring_Taskname ?? "", margin, yPoint, taskNameWidth, cellHeight);
            DrawTableCell(graphics, font, "  " + task.hiring_Description ?? "", margin + taskNameWidth, yPoint, descriptionWidth, cellHeight, wrapText: true);

            yPoint += cellHeight;

            // ? Check if new page is needed
            if (yPoint + cellHeight > page.Height - margin)
            {
                page = document.AddPage();
                graphics = XGraphics.FromPdfPage(page);
                yPoint = margin + 20;

                // ? Redraw header
                DrawTableCell(graphics, font, "Task Name", margin, yPoint, taskNameWidth, cellHeight, isHeader: true);
                DrawTableCell(graphics, font, "Description", margin + taskNameWidth, yPoint, descriptionWidth, cellHeight, isHeader: true);
                yPoint += cellHeight;
            }
        }

        // ? Table border
        graphics.DrawRectangle(XPens.Black, margin, margin + 20, pageWidth, yPoint - (margin + 20));

        document.Save(ms);
        return ms.ToArray();
    }

    private void DrawTableCell(XGraphics graphics, XFont font, string text, double x, double y, double width, double height, bool isHeader = false, bool wrapText = false)
    {
        // ? Cell border
        graphics.DrawRectangle(XPens.Black, x, y, width, height);

        if (isHeader)
        {
            graphics.DrawRectangle(XBrushes.Black, x, y, width, height);
            graphics.DrawString(text, font, XBrushes.White, new XRect(x, y, width, height), XStringFormats.Center);
        }
        else
        {
            if (wrapText)
            {
                var textFormatter = new XTextFormatter(graphics);
                textFormatter.DrawString(text, font, XBrushes.Black, new XRect(x, y, width, height), XStringFormats.TopLeft);
            }
            else
            {
                graphics.DrawString(text, font, XBrushes.Black, new XRect(x, y, width, height), XStringFormats.Center);
            }
        }
    }

    private void DrawTableRowBackground(XGraphics graphics, bool isStriped, double x, double y, double width, double height)
    {
        if (isStriped)
        {
            graphics.DrawRectangle(XBrushes.LightGray, x, y, width, height);
        }
    }

    private void SetSdtElementText(SdtElement sdt, string text)
    {
        Run run = sdt.Descendants<Run>().FirstOrDefault()!;
        if (run != null)
        {
            Text textElement = run.Descendants<Text>().FirstOrDefault()!;
            if (textElement != null)
            {
                textElement.Text = text;
            }
            else
            {
                run.AppendChild(new Text(text));
            }
        }
        else
        {
            sdt.AppendChild(new SdtContentRun(new Run(new Text(text))));
        }
    }
}
