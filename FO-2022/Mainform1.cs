using FO_2022.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FO_2022
{
    public partial class Mainform1 : Form
    {
        public bool isAdmin = false;
        public Mainform1()
        {
            InitializeComponent();
        }
        //private void Mainfrom_Load(object sender, EventArgs e)
        //{
        //    if (!isAdmin)
        //    {
        //        // Admin-only buttons hide
        //        userManagementBtn.Visible = false;  // Example: user management button
        //        settingsBtn.Visible = false;        // Example: settings button
        //    }
        //    else
        //    {
        //        // Admin ke liye full access
        //        userManagementBtn.Visible = true;
        //        settingsBtn.Visible = true;
        //    }
        //}
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            //this.WindowState = (this.WindowState == FormWindowState.Maximized)
            //     ? FormWindowState.Normal
            //     : FormWindowState.Maximized;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        //private void OpenForm(Form form)
        //{
        //    form.Show();
        //    this.Close();
        //}
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            SaleInvoice si = new SaleInvoice();
            si.Show();
            ////this.Hide();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            PurchaseInvoice pi = new PurchaseInvoice();
            pi.Show();
            ////this.Hide();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {    
              Party_Detailed_Ledger_Combine srr=new Party_Detailed_Ledger_Combine();
            srr.Show();
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            Cash_Book cb = new Cash_Book();
            cb.Show();
            ////this.Hide();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Customer_Receivables_By_Aging cra = new Customer_Receivables_By_Aging();
            cra.Show();
            ////this.Hide();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Party_Product_Sales_Ledger ppsl = new Party_Product_Sales_Ledger();
            ppsl.Show();
            ////this.Hide();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            Party_Product_Purchase_Ledger pppl = new Party_Product_Purchase_Ledger();
            pppl.Show();
            ////this.Hide();
        }
        private void pictureBox9_Click(object sender, EventArgs e)
        {
            Sale_Register_1_ srr=new Sale_Register_1_();
            srr.Show();
        }
        private void pictureBox10_Click(object sender, EventArgs e)
        {
            Sales_Summary_Product_Wise sspw = new Sales_Summary_Product_Wise();
            sspw.Show();
            ////this.Hide();
        }
        private void pictureBox11_Click(object sender, EventArgs e)
        {
            Accounts_Ledger ac = new Accounts_Ledger();
            ac.Show();
            ////this.Hide();
        }
        private void pictureBox12_Click(object sender, EventArgs e)
        {
            Customer_Receivable_C_W cr = new Customer_Receivable_C_W();
            cr.Show();
            ////this.Hide();
        }
        private void pictureBox14_Click(object sender, EventArgs e)
        {
            Current_Stock cs = new Current_Stock();
            cs.Show();
            //this.Hide();
        }
        private void pictureBox15_Click(object sender, EventArgs e)
        {
            Sales_Recvery__Invoice_Voice_ sri = new Sales_Recvery__Invoice_Voice_();
            sri.Show();
            ////this.Hide();
        }
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Party_Product_Sales_Ledger ppsl = new Party_Product_Sales_Ledger();
            ppsl.Show();
            ////this.Hide();
        }
        private void saleOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sale_Order so = new Sale_Order();
            so.Show();
            ////this.Hide();
        }
        private void saleOrderApprovalsProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void retailToolStripMenuItem_Click(object sender, EventArgs e)
        {
           SaleInvoice si = new SaleInvoice();
            si.Show();
            ////this.Hide();
        }

        private void saleInvoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaleInvoice ci = new SaleInvoice();
            ci.Show();
            ////this.Hide();
        }

        private void purchaseInvoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PurchaseInvoice pi = new PurchaseInvoice();
            pi.Show();
            ////this.Hide();
        }

        private void purchaseInvoiceShortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Purchase_Invoice_01 pin = new Purchase_Invoice_01();
            pin.Show();
            ////this.Hide();
        }

        private void saleReturnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sale_Return_Invoice sri = new Sale_Return_Invoice();
            sri.Show();
            ////this.Hide();    
        }

        private void purchaseReturnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Purchase_Returns_Summary prs=new Purchase_Returns_Summary();
            //Purchase_Returns_Invoice prs=new Purchase_Returns_Invoice();
            Purhcase_Return_Invoice prs = new Purhcase_Return_Invoice();
            prs.Show();
            ////this.Hide();
        }

        private void stockIssueToSalesmenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stock_Issue_to_Salesmen sis = new Stock_Issue_to_Salesmen();
            sis.Show();
            ////this.Hide();
        }

        private void stockTransferSalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stock_Receive_to_Salesmen srs = new Stock_Receive_to_Salesmen();
            srs.Show();
            ////this.Hide();
        }

        private void productReplacementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stock_Transfer_Salesmen_to_Salesmen stss = new Stock_Transfer_Salesmen_to_Salesmen();
            stss.Show();
            ////this.Hide();
        }

        private void purchaseDirectToSaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Change_Product_Prices cpp=new Change_Product_Prices();
            cpp.Show();
        }

        private void postShortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Purchase_Direct_to_Sale pds = new Purchase_Direct_to_Sale();
            pds.Show();
            ////this.Hide();
        }

        private void postShortStockSalesOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Post_Short_Stock_Order_Invoices psso = new Post_Short_Stock_Order_Invoices();
            psso.Show();
            //this.Hide();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Companies comp = new Companies();
            comp.Show();
            //this.Hide();
        }

        private void packingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Define_Packings dp = new Define_Packings();
            dp.Show();
            //this.Hide();
        }

        private void proToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Product_Group pg = new Product_Group();
            pg.Show();
            //this.Hide();
        }

        private void productSubGroupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Product_Sub_Groups psg = new Product_Sub_Groups();
            psg.Show();
            //this.Hide();
        }

        private void productsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stockalertlvltxtbox stockalertlvltxtbox = new stockalertlvltxtbox();
            stockalertlvltxtbox.Show();
            //this.Hide();
        }

        private void tradeOfferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Trade_Offers to = new Trade_Offers();
            to.Show();
            //this.Hide();
        }

        private void changeProductPricesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Change_Product_Prices cpp = new Change_Product_Prices();
            cpp.Show();
            //this.Hide();
        }

        private void tradeOfferCustomerWiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Trade_Offer_Customer_Wise tocw = new Trade_Offer_Customer_Wise();
            tocw.Show();
            //this.Hide();
        }

        private void transportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Transport_Defination td = new Transport_Defination();
            td.Show();
            //this.Hide();
        }

        private void townsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Towns t = new Towns();
            t.Show();
            //this.Hide();
        }

        private void sectorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sectors sec = new Sectors();
            sec.Show();
            //this.Hide();
        }

        private void customerdCategrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Define_Customer_Category dfc = new Define_Customer_Category();
            dfc.Show();
            //this.Hide();
        }

        private void customersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Customers cus = new Customers();
            cus.Show();
            //this.Hide();
        }

        private void distributorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Distributor_s_Parties_Management dpm = new Distributor_s_Parties_Management();
            dpm.Show();
            //this.Hide();
        }

        private void vendorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Vendors vendors = new Vendors();
            vendors.Show();
            //this.Hide();
        }

        private void salesmanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Salesman salesman = new Salesman();
            salesman.Show();
            //this.Hide();
        }

        private void employeesBioDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Employees_Bio_Data employees_Bio_Data = new Employees_Bio_Data();
            employees_Bio_Data.Show();
            //this.Hide();
        }

        private void assignCustomersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Assign_Customers_to_Salesman assign_Customers_To_Salesman = new Assign_Customers_to_Salesman();
            assign_Customers_To_Salesman.Show();
            //this.Hide();
        }

        private void weeklyPaymentScheduleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Weekly_Payment_Schedule wps = new Weekly_Payment_Schedule();
            wps.Show();
            //this.Hide();
        }
        private void accountsCodeManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void advanceDefinationToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void cashReceivingVoucherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Debit_Voucher db = new Debit_Voucher();
            db.Show();
        }
        private void cashPaymentVoucherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Credit_Voucher cpv = new Credit_Voucher();
            cpv.Show();
            //this.Hide();
        }
        private void journalVoucherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Journal_Voucher jv = new Journal_Voucher();
            jv.Show();
            //this.Hide();
        }
        private void salesRecoveryInvoiceWiseAutoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sales_Recvery__Invoice_Voice_ sri = new Sales_Recvery__Invoice_Voice_();
            sri.Show();
            //this.Hide();
        }
        private void preToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void dailyRecoveryAndPaymentSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Daily_Recovery_and_Payment_Summary drps = new Daily_Recovery_and_Payment_Summary();
            drps.Show();
            //this.Hide();
        }
        private void issueCheckBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bank_Cheques_Issue bb=new Bank_Cheques_Issue();
            bb.Show();
        }
        private void bankChequeIssuingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bank_Cheques_Issue bci = new Bank_Cheques_Issue();
            bci.Show();
            //this.Hide();
        }
        private void bankChequeReconcilationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bank_Cheques_Reconciliation bcr = new Bank_Cheques_Reconciliation();
            bcr.Show();
            //this.Hide();
        }
        private void cashDepositInBankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cash_Deposit_in_Bank cdb = new Cash_Deposit_in_Bank();
            cdb.Show();
            //this.Hide();
        }
        private void chequeDepositInBankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cheque_Deposit_in_Bank chdb = new Cheque_Deposit_in_Bank();
            chdb.Show();
            //this.Hide();
        }
        private void depositReconcilationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Deposit_Reconciliation dr = new Deposit_Reconciliation();
            dr.Show();
            //this.Hide();
        }
        private void postDatedRecoverPromisesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Post_Date_Recovery_Promises pdrp = new Post_Date_Recovery_Promises();
            pdrp.Show();
            //this.Hide();
        }

        private void postDatesPaymentPromisesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Post_Date_Recovery_Promises pdpp = new Post_Date_Recovery_Promises();
            pdpp.Show();
        }
        private void promisesProcessingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Promises_Processing pp = new Promises_Processing();
            pp.Show();
            //this.Hide();
        }
        private void salesReportToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void saleReturnReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void profitReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void salesmenRepToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void activityReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void purchaseReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void purchaseReturnReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void stockReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void listReportsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void accountsListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Chart_of_Accounts ca = new Chart_of_Accounts();
            ca.Show();
            ////this.Hide();
        }
        private void openingBalancesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Accounts_Balancing apb = new Accounts_Balancing();
            apb.Show();
            ////this.Hide();
        }
        private void accountLedgerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Accounts_Ledger al = new Accounts_Ledger();
            al.Show();
            //this.Hide();
        }
        private void salesmenPartyDetailLedgerCombineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Party_Detailed_Ledger_Combine pdlc = new Party_Detailed_Ledger_Combine();
            pdlc.Show();
            //this.Hide();
        }
        private void partyDetailedLedgerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Party_List_Detailed_1 pdl = new Party_List_Detailed_1();
            pdl.Show();
            //this.Hide();
        }
        private void partyLedgerForDailyActivePartiesExportPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Daily_activity_Done_Parties_Ledger padpl = new Daily_activity_Done_Parties_Ledger();
            padpl.Show();
            //this.Hide();
        }
        private void cashBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cash_Book cb = new Cash_Book();
            cb.Show();
            //this.Hide();
        }
        private void dayBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Day_Book db = new Day_Book();
            db.Show();
            //this.Hide();
        }
        private void bankJournalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Journal_Voucher jv = new Journal_Voucher();
            jv.Show();
            //this.Hide();
        }
        private void gLJournalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            General_Journal gj = new General_Journal();
            gj.Show();
            //this.Hide();
        }
        private void accountsReceivablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Account_Receivable ar = new Account_Receivable();
            ar.Show();
            //this.Hide();
        }
        private void accountsPayableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Account_Payable ap = new Account_Payable();
            ap.Show();
        }
        private void monthlyExpenseChartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Monthly_Expense_Chart mec = new Monthly_Expense_Chart();
            mec.Show();
            //this.Hide();
        }
        private void bankStatementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bank_Statement bs = new Bank_Statement();
            bs.Show();
            //this.Hide();
        }
        private void bankMarkupCalculatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bank_Marup_Calculator bmc = new Bank_Marup_Calculator();
            bmc.Show();
            //this.Hide();
        }

        private void bouncedChequesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Uncleared_Cheques cq=new Uncleared_Cheques();
            cq.Show();
        }
        private void lostChequesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Uncleared_Cheques cd=new Uncleared_Cheques();
            cd.Show();
        }
        private void unclearedChequesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Uncleared_Cheques uc = new Uncleared_Cheques();
            uc.Show();
            //this.Hide();
        }

        private void accountBalancesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Account_Balances ab = new Account_Balances();
            ab.Show();
            //this.Hide();
        }
        private void balanceSheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Balance_Sheet_Reporting_Setting bsrs = new Balance_Sheet_Reporting_Setting();
            Balance_Sheet bsrs = new Balance_Sheet();
            bsrs.Show();
            //this.Hide();
        }
        private void trialBalanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Trial_Balances tb = new Trial_Balances();
            tb.Show();
            //this.Hide();
        }

        private void profitAndLossStatementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Profit_and_Loss_Statement pls = new Profit_and_Loss_Statement();
            pls.Show();
            //this.Hide();
        }

        private void logOffFurqanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void dailyClosingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void systemSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System_Settings ss = new System_Settings();
            ss.Show();
        }
        private void backupOnHardDriveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Data_Backup_On_Hard_Drive bhd = new Data_Backup_On_Hard_Drive();
            bhd.Show();
            //this.Hide();
        }
        private void backupOnDriveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Data_Backup_On_USB_Drive bud = new Data_Backup_On_USB_Drive();
            bud.Show();
            //this.Hide();
        }
        private void userSecurityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Create_New_Users cup = new Create_New_Users();
            cup.Show();
        }
        private void openingBalancesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Accounts_Opening_Balance aob = new Accounts_Opening_Balance();
            aob.Show();
            //this.Hide();
        }
        private void updateCostForPurchasedProductToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cost_Update_for_Purchase_Product cupp = new Cost_Update_for_Purchase_Product();
            cupp.Show();
            //this.Hide();
        }
        private void openingStockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Opening_stock os = new Opening_stock();
            os.Show();
            //this.Hide();
        }
        private void balanceSheetSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Balance_Sheet_Reporting_Setting bsrs = new Balance_Sheet_Reporting_Setting();
            bsrs.Show();
            //this.Hide();
        }
        private void profitAndLossSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Profit_and_Loss_Report_Setting pls = new Profit_and_Loss_Report_Setting();
            pls.Show();
            //this.Hide();
        }
        private void expenseReportSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Expense_Report_Setting ers = new Expense_Report_Setting();
            ers.Show();
            //this.Hide();
        }
        private void updateDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();        }
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void companyNSMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Define_Company_NSM nsm= new Define_Company_NSM();
            nsm.Show();
            //this.Hide();
        }
        private void companyASMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ASM_Form asm = new ASM_Form();
            asm.Show();
            //this.Hide();
        }
        private void companySOsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SO_s_ASM_Wise so = new SO_s_ASM_Wise();
            so.Show();
            //this.Hide();
        }

        private void logoutlbl_Click(object sender, EventArgs e)
        {
          Application.Exit();

        }

        private void toolStripSplitButton5_ButtonClick(object sender, EventArgs e)
        {

        }

        private void toolStripSplitButton4_ButtonClick(object sender, EventArgs e)
        {
            toolStripSplitButton4.ShowDropDown();
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            toolStripSplitButton1.ShowDropDown();
        }

        private void toolStripSplitButton2_ButtonClick(object sender, EventArgs e)
        {
            toolStripSplitButton2.ShowDropDown();
        }

        private void toolStripSplitButton3_ButtonClick(object sender, EventArgs e)
        {
            toolStripSplitButton3.ShowDropDown();
        }

        private void toolStripSplitButton5_ButtonClick_1(object sender, EventArgs e)
        {
            toolStripSplitButton5.ShowDropDown();
        }

        private void toolStripSplitButton7_ButtonClick(object sender, EventArgs e)
        {
            toolStripSplitButton7.ShowDropDown();
        }

        private void toolStripSplitButton8_ButtonClick(object sender, EventArgs e)
        {
            toolStripSplitButton8.ShowDropDown();
        }

        private void partySalesActivityAgringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Party_Sale_Aging sg = new Party_Sale_Aging();
            sg.Show();
        }

        private void dailyPendingSalesOrdersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //not found
        }

        private void builtyRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //not found
        }

        private void salesRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sale_Register_1_ srr = new Sale_Register_1_();
            srr.Show();
        }

        private void salesRegisterCompanyWiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sales_Register_Company_Wise srcw = new Sales_Register_Company_Wise();
            srcw.Show();

        }

        private void salesSummaryInvoiceWiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            // not found
        }

        private void salesSummaryProductWiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sales_Summary_Product_Wise asr = new Sales_Summary_Product_Wise();
            asr.Show();
        }

        private void partyProductWiseSaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // not found
            Product_Wise_Sale swp = new Product_Wise_Sale();
            swp.Show();
        }

        private void partyProductSaleLedgerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Party_Product_Sales_Ledger pps = new Party_Product_Sales_Ledger();
            pps.Show();
        }

        private void partySalesRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sale_Register_1_ sale_Register_1_ = new Sale_Register_1_();
            sale_Register_1_.Show();
            // not found
        }

        private void salesVsRecoveriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sales_VS_Recoveries sr = new Sales_VS_Recoveries();
            sr.Show();
        }

        private void salesVsRecoveries2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sale_vs_Recovery sdd=new Sale_vs_Recovery();
               sdd.Show();
            // not found
            //Sales
        }

        private void salesComparisonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sales_Month_Comparison_Product_Wise sm = new Sales_Month_Comparison_Product_Wise();
            sm.Show();
        }

        private void salesComparisonDistributorWiseProductsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sales_Month_Comparison_Distributors_Wise_Products smr = new Sales_Month_Comparison_Distributors_Wise_Products();
            smr.Show();
        }

        private void quarterlySalesPartyWiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Quaterely_Sales_Party_Wise qspw = new Quaterely_Sales_Party_Wise();
            qspw.Show();
        }

        private void companyWiseSalesToolStripMenuItem_Click(object sender, EventArgs e)
        { 
            Company_Wise_Sales cws = new Company_Wise_Sales();
            cws.Show();
        }

        private void discountClaimSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // not found
            Company_Discount_Claim_Summary cdcs = new Company_Discount_Claim_Summary();
            cdcs.Show();
        }

        private void companyDiscountClaimSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Company_Discount_Claim_Summary sw = new Company_Discount_Claim_Summary();
            sw.Show();
        }

        private void categoryWiseSaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            

        }

        private void productSalesLedgerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Product_Sale_Ledger sp = new Product_Sale_Ledger();
            sp.Show();
        }

        private void customerTownSectorWiseSalesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Customer_Town_and_Sector_Wise_Sales sr = new Customer_Town_and_Sector_Wise_Sales();
            sr.Show();
        }

        private void salesAndStockStatementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sales_and_Stock_Statement ss = new Sales_and_Stock_Statement();
            ss.Show();
        }

        private void saleReturnRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sales_Register_Company_Wise sr = new Sales_Register_Company_Wise();
            sr.Show();
        }

        private void saleReturnSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sale_Return_Summary ss = new Sale_Return_Summary();
            ss.Show();
        }

        private void productSaleReturnLedgerToolStripMenuItem_Click(object sender, EventArgs e)
        {
                Product_Sale_Ledger  pss=new Product_Sale_Ledger();
                    pss.Show();
        }

        private void salesmanSummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Salesman_Summary ss = new Salesman_Summary();
            ss.Show();
        }

        private void salesmanProductsWiseSaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Product_Wise_Sales_1 ss = new Product_Wise_Sales_1();
            ss.Show();
        }

        private void salesmanMonthlyPerformanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Salesman_Monthly_Performance pm = new Salesman_Monthly_Performance();
            pm.Show();
        }

        private void salesmanCommissionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Salesman_Commision sm = new Salesman_Commision();
            sm.Show();
        }

        private void salesmanStockStatementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Salesman_Stock_Statement ss = new Salesman_Stock_Statement();
            ss.Show();
        }

        private void dailySaleStatementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Daily_Sales_Statement_Salesman_Wise_ ss = new Daily_Sales_Statement_Salesman_Wise_();
            ss.Show();
        }

        private void salesmanCompanyWiseProductsSaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Product_Wise_Sales_1 s=new Product_Wise_Sales_1();
            //s.Show();
            Product_Wise_Sales_1 sq = new Product_Wise_Sales_1();
            sq.Show();

        }

        private void salesmanSalesAndCashActivityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Salesman_Stock_Statement ww = new Salesman_Stock_Statement();
            ww.Show();
        }

        private void dailyTramactionReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Daily_Transaction_Report dt = new Daily_Transaction_Report();
            dt.Show();
        }

        private void dailyBusinessActivityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Daily_Business_Activity db = new Daily_Business_Activity();
            db.Show();
        }

        private void customerReceivablesinvoiceWiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Customer_Receivables__Invoice_Wise_ crs = new Customer_Receivables__Invoice_Wise_();
            crs.Show();
        }

        private void customerReceivablesCategoryWiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Customer_Receivable_C_W ccc = new Customer_Receivable_C_W();
            ccc.Show();
            
        }

        private void custornerReceivablesByAgeingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Customer_Receivables_By_Aging ag = new Customer_Receivables_By_Aging();
            ag.Show();
        }

        private void customerPayableByAgeingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //not fpund
            Due_Recovery_By_Credit_Days ss = new Due_Recovery_By_Credit_Days();
            ss.Show();
        }

        private void dueAgeingReceivablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Due_Recovery_By_Credit_Days ss = new Due_Recovery_By_Credit_Days ();
            ss.Show();
        }

        private void deadPartiesDueBalancesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dead_Parties_Due_Balance dp = new Dead_Parties_Due_Balance();
            dp.Show();
        }

        private void accountBalanceCompanyWieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Account_Balance__C_W cw = new Account_Balance__C_W();
            cw.Show();
        }

        private void customerReceivablesCompanyWiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Customer_Receivables__Invoice_Wise_ cw = new Customer_Receivables__Invoice_Wise_();
            cw.Show();
        }

        private void productActivityReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Product_Activity_Report ac = new Product_Activity_Report();
            ac.Show();
        }

        private void companyProductActivityReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //not fpund
        }

        private void salesRecoveryRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sales_Recovery_Register str = new Sales_Recovery_Register();
            str.Show();
        }

        private void continuesInvoicePrintingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Accounts_Ledger al = new Accounts_Ledger();
            al.Show();
        }

        private void accountLedgerDateWoeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Accounts_Ledger_1 AC = new Accounts_Ledger_1();
            AC.Show();
        }

        private void purchaseDateWiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Purchase_Registerr_Date_Wise dw = new Purchase_Registerr_Date_Wise();
            dw.Show();
        }

        private void purchaseInvoiceWiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Purchase_Register_Invoice_Wise inh = new Purchase_Register_Invoice_Wise();
            inh.Show();
        }

        private void purchaseSummaryReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Purchase_Summary ss = new Purchase_Summary();
            ss.Show();
        }

        private void productWisePurchaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Product_Wise_Purchase an = new Product_Wise_Purchase();
            //an.Show();
            }

        private void purchaseReturnRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Purchase_Return_Register prr = new Purchase_Return_Register();
            prr.Show();
        }

        private void purchaseReturnSumaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Purchase_Returns_Summary purchase_Returns_Summary = new Purchase_Returns_Summary();
            purchase_Returns_Summary.Show();
        }

        private void productPurchaseReturnLedgerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Product_Purchase_Return_Ledger ppl = new Product_Purchase_Return_Ledger();
            ppl.Show();
        }

        private void currentStockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Current_Stock cs = new Current_Stock();
            cs.Show();
        }

        private void openingStockToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Opening_Stock_1 os = new Opening_Stock_1();
            os.Show();
        }

        private void stockGoingToShortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stock_Going_to_Short sg = new Stock_Going_to_Short();
            sg.Show();

        }

        private void salesmanCurrentStockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Salesman_Stock_Statement ss = new Salesman_Stock_Statement();
            ss.Show();
        }

        private void productPurchaseLedgerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Party_Product_Purchase_Ledger lp = new Party_Product_Purchase_Ledger();
            lp.Show();
        }
        private void purchaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Purchase_Month_Comparison_Product_Wise mp = new Purchase_Month_Comparison_Product_Wise();
            mp.Show();
        }

        private void stockIssueRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stock_Issue_Register si = new Stock_Issue_Register();
            si.Show();
        }

        private void stockReturnRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stock_Return_Register sr = new Stock_Return_Register();
            sr.Show();
        }

        private void employeesSalarySheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Employees_Salary_Sheet ess = new Employees_Salary_Sheet();
            ess.Show();
        }

        private void salesmanPartiesListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Party_Detailed_Ledger_Combine party_Detailed_Ledger_Combine = new Party_Detailed_Ledger_Combine();
            party_Detailed_Ledger_Combine.Show();
        }

        private void townsListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Town_List tt = new Town_List();
            tt.Show();
        }

        private void sectorsListAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sectors_List_A11 sec = new Sectors_List_A11 ();
            sec.Show();
        }

        private void sectorsListSalesmanWiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Salesman_Sector_List sl = new Salesman_Sector_List();
            sl.Show();
        }

        private void partyListSummarizedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Party_List_Summarize pls = new Party_List_Summarize();
            pls.Show();
        }

        private void partyListDetailedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Party_List_Detailed_1 mn = new Party_List_Detailed_1();
        mn.Show();
        }

        private void stockCountSheetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stock_Current_Sheet scs = new Stock_Current_Sheet();
            scs.Show();
        }

        private void productPriceListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Product_Price_List ppl = new Product_Price_List();
            ppl.Show();
        }

        private void deadPartyListReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dead_Parties dp=new Dead_Parties();
            dp.Show();
        }

        private void companyWiseProfitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Company_Wise_Profits sd=new Company_Wise_Profits();
            sd.Show();
        }

        private void customerWiseProfitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Customer_Wise_Profits customer_Wise_Profits = new Customer_Wise_Profits();
            customer_Wise_Profits.Show();
        }

        private void dailyProfitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Daily_Profits dp = new Daily_Profits();
            dp.Show();
        }

        private void invoiceDetailedProfitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Invoice_Detailed_Profit sd = new Invoice_Detailed_Profit();
            sd.Show();
        }

        private void invoiceWIseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Invoice_Wise_Profit invoice_Wise_Profit = new Invoice_Wise_Profit();
            invoice_Wise_Profit.Show();
        }

        private void productWiseProfitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Product_Wise_Profits product_Wise_Profits = new Product_Wise_Profits();
            product_Wise_Profits.Show();
        }

        private void salesMonthComparisonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sales_Month_Comparison_Salesman_Distributors_Wise_Product ss=new Sales_Month_Comparison_Salesman_Distributors_Wise_Product();
            ss.Show();
        }

        private void customerInvoiceWiseSaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Customer_Invoice_Wise_Sales s = new Customer_Invoice_Wise_Sales();
            s.Show();
        }

        private void customerRecoveryAverageInDAysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Season_Recovery ssr = new Season_Recovery();
            ssr.Show();
        }

        private void partyTransactionWeekDaysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Accounts_Ledger_1 accounts_Ledger_1 = new Accounts_Ledger_1();
            accounts_Ledger_1.Show();
        }

        private void purchaseInvoiceShortToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Purchase_Invoice_01 pp = new Purchase_Invoice_01();
            pp.Show();

        }
    }
}
