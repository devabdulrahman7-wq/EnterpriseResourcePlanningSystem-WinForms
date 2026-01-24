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
            Application.Exit();        }
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
            PurchaseInvoice pi=new PurchaseInvoice();
            pi.Show();
            ////this.Hide();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Sale_Register ins = new Sale_Register();
            ins.Show();
            ////this.Hide();
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            Cash_Book cb = new Cash_Book();
            cb.Show();
            ////this.Hide();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Customer_Receivables_By_Aging cra=new Customer_Receivables_By_Aging();
            cra.Show();
            ////this.Hide();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Party_Product_Sales_Ledger ppsl=new Party_Product_Sales_Ledger();
            ppsl.Show();
            ////this.Hide();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            Party_Product_Purchase_Ledger pppl=new Party_Product_Purchase_Ledger();
            pppl.Show();
            ////this.Hide();
        }
        private void pictureBox9_Click(object sender, EventArgs e)
        {
            Sales_Register sr = new Sales_Register();
            sr.Show();
            ////this.Hide();
        }
        private void pictureBox10_Click(object sender, EventArgs e)
        {
            Sales_Summary_Product_Wise sspw=new Sales_Summary_Product_Wise();
            sspw.Show();
            ////this.Hide();
        }
        private void pictureBox11_Click(object sender, EventArgs e)
        {
            Accounts_Ledger ac=new Accounts_Ledger();
            ac.Show();
            ////this.Hide();
        }
        private void pictureBox12_Click(object sender, EventArgs e)
        {
           Customer_Receivable_C_W cr=new Customer_Receivable_C_W();
            cr.Show();
            ////this.Hide();
        }
        private void pictureBox14_Click(object sender, EventArgs e)
        {
            Current_Stock cs=new Current_Stock();
            cs.Show();
            //this.Hide();
        }
        private void pictureBox15_Click(object sender, EventArgs e)
        {
            Sales_Recvery__Invoice_Voice_ sri=new Sales_Recvery__Invoice_Voice_();
            sri.Show();
            ////this.Hide();
        }
        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Party_Product_Sales_Ledger ppsl=new Party_Product_Sales_Ledger();
            ppsl.Show();
            ////this.Hide();
        }
        private void saleOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sale_Order so=new Sale_Order();
            so.Show();
            ////this.Hide();
        }
        private void saleOrderApprovalsProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
        private void retailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void saleInvoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaleInvoice ci=new SaleInvoice();
            ci.Show();  
            ////this.Hide();
        }

        private void purchaseInvoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Purchase_Invoice_01 pi=new Purchase_Invoice_01();
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
            Sale_Return_Invoice sri=new Sale_Return_Invoice();
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
            Stock_Receive_to_Salesmen srs=new Stock_Receive_to_Salesmen();
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
            
        }

        private void postShortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Purchase_Direct_to_Sale pds = new Purchase_Direct_to_Sale();
            pds.Show();
            ////this.Hide();
        }

        private void postShortStockSalesOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Post_Short_Stock_Order_Invoices psso=new Post_Short_Stock_Order_Invoices();
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
            Define_Packings dp=new Define_Packings();
            dp.Show();
            //this.Hide();
        }

        private void proToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Product_Group pg=new Product_Group();
            pg.Show();
            //this.Hide();
        }

        private void productSubGroupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Product_Sub_Groups psg=new Product_Sub_Groups();
            psg.Show();
            //this.Hide();
        }

        private void productsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stockalertlvltxtbox  stockalertlvltxtbox=new stockalertlvltxtbox ();
            stockalertlvltxtbox .Show();
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
           Trade_Offer_Customer_Wise tocw=new Trade_Offer_Customer_Wise();
            tocw.Show();
            //this.Hide();
        }

        private void transportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Transport_Defination td=new Transport_Defination();
            td.Show();
            //this.Hide();
        }

        private void townsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Town_List t=new Town_List();
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
            Define_Customer_Category dfc=new Define_Customer_Category();
            dfc.Show();
            //this.Hide();
        }

        private void customersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Customers cus=new Customers();
            cus.Show();
            //this.Hide();
        }

        private void distributorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Distributor_s_Parties_Management dpm=new Distributor_s_Parties_Management();
            dpm.Show();
            //this.Hide();
        }

        private void vendorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Vendors vendors=new Vendors();
            vendors.Show();
            //this.Hide();
        }

        private void salesmanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Salesman salesman=new Salesman();
            salesman.Show();
            //this.Hide();
        }

        private void employeesBioDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Employees_Bio_Data employees_Bio_Data= new Employees_Bio_Data();
            employees_Bio_Data.Show();
            //this.Hide();
        }

        private void assignCustomersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Assign_Customers_to_Salesman assign_Customers_To_Salesman=new Assign_Customers_to_Salesman();
            assign_Customers_To_Salesman.Show();
            //this.Hide();
        }

        private void weeklyPaymentScheduleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Weekly_Payment_Schedule wps=new Weekly_Payment_Schedule();
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
            
        }
        private void cashPaymentVoucherToolStripMenuItem_Click(object sender, EventArgs e)
        {
           Cash_Payment_Voucher cpv=new Cash_Payment_Voucher();
            cpv.Show();
            //this.Hide();
        }
        private void journalVoucherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Journal_Voucher jv=new Journal_Voucher();
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
            Daily_Recovery_and_Payment_Summary drps=new Daily_Recovery_and_Payment_Summary();
            drps.Show();
            //this.Hide();
        }
        private void issueCheckBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }
        private void bankChequeIssuingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bank_Cheques_Issue bci = new Bank_Cheques_Issue();
            bci.Show();
            //this.Hide();
        }
        private void bankChequeReconcilationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bank_Cheques_Reconciliation bcr=new Bank_Cheques_Reconciliation();
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
            Accounts_Balancing apb= new Accounts_Balancing();
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
            Party_Detailed_Ledger_Combine pdlc=new Party_Detailed_Ledger_Combine();
            pdlc.Show();
            //this.Hide();
        }
        private void partyDetailedLedgerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Party_Deatail_Ledger pdl = new Party_Deatail_Ledger();
           pdl.Show();
            //this.Hide();
        }
        private void partyLedgerForDailyActivePartiesExportPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Daily_activity_Done_Parties_Ledger padpl=new Daily_activity_Done_Parties_Ledger();
            padpl.Show();
            //this.Hide();
        }
        private void cashBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cash_Book cb=new Cash_Book();
            cb.Show();
            //this.Hide();
        }
        private void dayBookToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Day_Book db=new Day_Book();
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
            
        }
        private void monthlyExpenseChartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Monthly_Expense_Chart mec = new Monthly_Expense_Chart();
            mec.Show();
            //this.Hide();
        }
        private void bankStatementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bank_Statement bs=new Bank_Statement();
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
           
        }
        private void lostChequesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
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
            Trial_Balances tb=new Trial_Balances();
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

        }

        private void dailyClosingToolStripMenuItem_Click(object sender, EventArgs e)
        {
         
        }

        private void systemSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
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
            Profit_and_Loss_Statement pls = new Profit_and_Loss_Statement();
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
            this.Close();
        }
       private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void companyNSMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Customer_Wise_Profits nsm =new Customer_Wise_Profits();
            nsm.Show();
            //this.Hide();
        }
        private void companyASMToolStripMenuItem_Click(object sender, EventArgs e)
        {
           ASM_Form asm=new ASM_Form();
            asm.Show();
            //this.Hide();
        }
        private void companySOsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Company_Wise_Profits so=new Company_Wise_Profits();  
            so.Show();
            //this.Hide();
        }

        private void logoutlbl_Click(object sender, EventArgs e)
        {
            Mainform1 mm = new Mainform1();
            this.Hide();
            Login lg = new Login();
            lg.Show();

        }

        private void toolStripSplitButton5_ButtonClick(object sender, EventArgs e)
        {

        }
    }
}
