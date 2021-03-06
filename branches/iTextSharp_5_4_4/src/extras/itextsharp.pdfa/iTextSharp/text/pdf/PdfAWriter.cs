using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text.error_messages;
using iTextSharp.text.log;
using iTextSharp.text.pdf.interfaces;
using iTextSharp.text.xml.xmp;
using iTextSharp.text.pdf.intern;
/*
 * $Id: PdfAWriter.java 5853 2013-06-13 13:21:03Z eugenemark $
 *
 * This file is part of the iText (R) project.
 * Copyright (c) 1998-2012 1T3XT BVBA
 * Authors: Alexander Chingarev, Bruno Lowagie, et al.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License version 3
 * as published by the Free Software Foundation with the addition of the
 * following permission added to Section 15 as permitted in Section 7(a):
 * FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY 1T3XT,
 * 1T3XT DISCLAIMS THE WARRANTY OF NON INFRINGEMENT OF THIRD PARTY RIGHTS.
 *
 * This program is distributed in the hope that it will be useful, but
 * WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU Affero General Public License for more details.
 * You should have received a copy of the GNU Affero General Public License
 * along with this program; if not, see http://www.gnu.org/licenses or write to
 * the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
 * Boston, MA, 02110-1301 USA, or download the license from the following URL:
 * http://itextpdf.com/terms-of-use/
 *
 * The interactive user interfaces in modified source and object code versions
 * of this program must display Appropriate Legal Notices, as required under
 * Section 5 of the GNU Affero General Public License.
 *
 * In accordance with Section 7(b) of the GNU Affero General Public License,
 * a covered work must retain the producer line in every PDF that is created
 * or manipulated using iText.
 *
 * You can be released from the requirements of the license by purchasing
 * a commercial license. Buying such a license is mandatory as soon as you
 * develop commercial activities involving the iText software without
 * disclosing the source code of your own applications.
 * These activities include: offering paid services to customers as an ASP,
 * serving PDFs on the fly in a web application, shipping iText with a closed
 * source product.
 *
 * For more information, please contact iText Software Corp. at this
 * address: sales@itextpdf.com
 */

namespace iTextSharp.text.pdf {
    /**
     * @see PdfWriter
     */

    public class PdfAWriter : PdfWriter {

        protected ICC_Profile colorProfile;

        /**
         * Use this method to get an instance of the <CODE>PdfWriter</CODE>.
         * @param	document	The <CODE>Document</CODE> that has to be written
         * @param	os	The <CODE>Stream</CODE> the writer has to write to.
         * @param conformanceLevel PDF/A conformance level of a new PDF document
         * @return	a new <CODE>PdfWriter</CODE>
         * @throws	DocumentException on error
         */

        public static PdfAWriter GetInstance(Document document, Stream os, PdfAConformanceLevel conformanceLevel) {
            PdfDocument pdf = new PdfDocument();
            document.AddDocListener(pdf);
            PdfAWriter writer = new PdfAWriter(pdf, os, conformanceLevel);
            pdf.AddWriter(writer);
            return writer;
        }

        /**
         * Use this method to get an instance of the <CODE>PdfWriter</CODE>.
         * @param	document	The <CODE>Document</CODE> that has to be written
         * @param	os	The <CODE>Stream</CODE> the writer has to write to.
         * @param listener A <CODE>DocListener</CODE> to pass to the PdfDocument.
         * @param conformanceLevel PDF/A conformance level of a new PDF document
         * @return	a new <CODE>PdfWriter</CODE>
         * @throws	DocumentException on error
         */

        public static PdfAWriter GetInstance(Document document, Stream os, IDocListener listener,
                                             PdfAConformanceLevel conformanceLevel) {
            PdfDocument pdf = new PdfDocument();
            pdf.AddDocListener(listener);
            document.AddDocListener(pdf);
            PdfAWriter writer = new PdfAWriter(pdf, os, conformanceLevel);
            pdf.AddWriter(writer);
            return writer;
        }

        /**
         *
         * @param writer
         * @param conformanceLevel
         */

        public static void SetPdfVersion(PdfWriter writer, PdfAConformanceLevel conformanceLevel) {
            switch (conformanceLevel) {
                case PdfAConformanceLevel.PDF_A_1A:
                case PdfAConformanceLevel.PDF_A_1B:
                    writer.PdfVersion = PdfWriter.VERSION_1_4;
                    break;
                case PdfAConformanceLevel.PDF_A_2A:
                case PdfAConformanceLevel.PDF_A_2B:
                case PdfAConformanceLevel.PDF_A_2U:
                    writer.PdfVersion = PdfWriter.VERSION_1_7;
                    break;
                case PdfAConformanceLevel.PDF_A_3A:
                case PdfAConformanceLevel.PDF_A_3B:
                case PdfAConformanceLevel.PDF_A_3U:
                    writer.PdfVersion = PdfWriter.VERSION_1_7;
                    break;
                default:
                    writer.PdfVersion = PdfWriter.VERSION_1_4;
                    break;
            }
        }

        /**
         * @see PdfWriter#setOutputIntents(String, String, String, String, ICC_Profile)
         */

        public override void SetOutputIntents(String outputConditionIdentifier, String outputCondition,
                                              String registryName, String info, ICC_Profile colorProfile) {
            base.SetOutputIntents(outputConditionIdentifier, outputCondition, registryName, info, colorProfile);
            PdfArray a = extraCatalog.GetAsArray(PdfName.OUTPUTINTENTS);
            if (a != null) {
                PdfDictionary d = a.GetAsDict(0);
                if (d != null)
                    d.Put(PdfName.S, PdfName.GTS_PDFA1);
            }
            this.colorProfile = colorProfile;
        }

        /**
         * Always throws an exception since PDF/X conformance level cannot be set for PDF/A conformant documents.
         * @param pdfx
         */

        public void SetPDFXConformance(int pdfx) {
            throw new PdfXConformanceException(
                MessageLocalization.GetComposedMessage("pdfx.conformance.cannot.be.set.for.PdfAWriter.instance"));
        }

        /**
         * @see com.itextpdf.text.pdf.PdfWriter#isPdfIso()
         */

        public override bool IsPdfIso() {
            return pdfIsoConformance.IsPdfIso();
        }

        public ICC_Profile GetColorProfile() {
            return colorProfile;
        }

        /**
         * @param conformanceLevel PDF/A conformance level of a new PDF document
         */

        protected internal PdfAWriter(PdfAConformanceLevel conformanceLevel)
            : base() {
            ((IPdfAConformance) pdfIsoConformance).SetConformanceLevel(conformanceLevel);
            SetPdfVersion(this, conformanceLevel);
        }

        /**
         * Constructs a <CODE>PdfAWriter</CODE>.
         * <P>
         * Remark: a PdfAWriter can only be constructed by calling the method <CODE>getInstance(Document document, Stream os, PdfAconformanceLevel conformanceLevel)</CODE>.
         * @param document the <CODE>PdfDocument</CODE> that has to be written
         * @param os the <CODE>Stream</CODE> the writer has to write to
         * @param conformanceLevel PDF/A conformance level of a new PDF document
         */

        protected internal PdfAWriter(PdfDocument document, Stream os, PdfAConformanceLevel conformanceLevel)
            : base(document, os) {
            ((IPdfAConformance) pdfIsoConformance).SetConformanceLevel(conformanceLevel);
            SetPdfVersion(this, conformanceLevel);
        }

        /**
         * @see com.itextpdf.text.pdf.PdfWriter#getTtfUnicodeWriter()
         */

        protected internal override TtfUnicodeWriter GetTtfUnicodeWriter() {
            if (ttfUnicodeWriter == null)
                ttfUnicodeWriter = new PdfATtfUnicodeWriter(this);
            return ttfUnicodeWriter;
        }

        override internal XmpWriter CreateXmpWriter(MemoryStream baos, PdfDictionary info) {
            return
                xmpWriter = new PdfAXmpWriter(baos, info, ((IPdfAConformance) pdfIsoConformance).ConformanceLevel);
        }

        override internal XmpWriter CreateXmpWriter(MemoryStream baos, IDictionary<String, String> info) {
            return
                xmpWriter = new PdfAXmpWriter(baos, info, ((IPdfAConformance) pdfIsoConformance).ConformanceLevel);
        }

        public override IPdfIsoConformance GetPdfIsoConformance() {
            return new PdfAConformanceImp(this);
        }

        protected ICounter COUNTER = CounterFactory.GetCounter(typeof (PdfAWriter));

        protected override ICounter GetCounter() {
            return COUNTER;
        }
    }

}
