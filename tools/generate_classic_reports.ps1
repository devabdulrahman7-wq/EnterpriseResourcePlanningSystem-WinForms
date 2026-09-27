$ErrorActionPreference = 'Stop'

$reportsDir = "e:\FO-2022\FO-2022\Reports"

function XmlEscape([string]$value) {
    if ($null -eq $value) { return "" }
    return [System.Security.SecurityElement]::Escape($value)
}

function New-FieldXml($field) {
    $name = XmlEscape $field.Name
    $dataField = XmlEscape $field.DataField
    $typeName = XmlEscape $field.TypeName
    return @"
        <Field Name="$name">
          <DataField>$dataField</DataField>
          <rd:TypeName>$typeName</rd:TypeName>
        </Field>
"@
}

function New-ParameterXml($paramName) {
    $paramName = XmlEscape $paramName
    return @"
    <ReportParameter Name="$paramName">
      <DataType>String</DataType>
      <Prompt>$paramName</Prompt>
    </ReportParameter>
"@
}

function New-ParagraphTextbox($name, $value, $align = "Left", $bold = $false, $borderColor = "Black", $fontSize = "8pt", $verticalAlign = "Top", $top = $null, $left = $null, $height = $null, $width = $null) {
    $name = XmlEscape $name
    $value = XmlEscape $value
    $fontWeight = if ($bold) { "<FontWeight>Bold</FontWeight>" } else { "" }
    $position = ""
    if ($top -ne $null) { $position += "<Top>$top</Top>" }
    if ($left -ne $null) { $position += "<Left>$left</Left>" }
    if ($height -ne $null) { $position += "<Height>$height</Height>" }
    if ($width -ne $null) { $position += "<Width>$width</Width>" }
    return @"
<Textbox Name="$name">
  <CanGrow>true</CanGrow>
  <KeepTogether>true</KeepTogether>
  <Paragraphs>
    <Paragraph>
      <TextRuns>
        <TextRun>
          <Value>$value</Value>
          <Style>
            <FontFamily>Arial</FontFamily>
            <FontSize>$fontSize</FontSize>
            $fontWeight
          </Style>
        </TextRun>
      </TextRuns>
      <Style>
        <TextAlign>$align</TextAlign>
      </Style>
    </Paragraph>
  </Paragraphs>
  $position
  <Style>
    <Border>
      <Color>$borderColor</Color>
      <Style>Solid</Style>
    </Border>
    <VerticalAlign>$verticalAlign</VerticalAlign>
    <PaddingLeft>2pt</PaddingLeft>
    <PaddingRight>2pt</PaddingRight>
    <PaddingTop>2pt</PaddingTop>
    <PaddingBottom>2pt</PaddingBottom>
  </Style>
</Textbox>
"@
}

function New-CellTextbox($name, $value, $align = "Left", $bold = $false, $borderColor = "Black", $verticalAlign = "Top") {
    $name = XmlEscape $name
    $value = XmlEscape $value
    $fontWeight = if ($bold) { "<FontWeight>Bold</FontWeight>" } else { "" }
    return @"
<Textbox Name="$name">
  <CanGrow>true</CanGrow>
  <KeepTogether>true</KeepTogether>
  <Paragraphs>
    <Paragraph>
      <TextRuns>
        <TextRun>
          <Value>$value</Value>
          <Style>
            <FontFamily>Arial</FontFamily>
            <FontSize>8pt</FontSize>
            $fontWeight
          </Style>
        </TextRun>
      </TextRuns>
      <Style>
        <TextAlign>$align</TextAlign>
      </Style>
    </Paragraph>
  </Paragraphs>
  <Style>
    <Border>
      <Color>$borderColor</Color>
      <Style>Solid</Style>
    </Border>
    <VerticalAlign>$verticalAlign</VerticalAlign>
    <PaddingLeft>2pt</PaddingLeft>
    <PaddingRight>2pt</PaddingRight>
    <PaddingTop>2pt</PaddingTop>
    <PaddingBottom>2pt</PaddingBottom>
  </Style>
</Textbox>
"@
}

function New-ClassicReport($spec) {
    $fieldXml = ($spec.Fields | ForEach-Object { New-FieldXml $_ }) -join "`r`n"
    $parameterXml = if ($spec.Parameters.Count -gt 0) {
        @"
  <ReportParameters>
$(
($spec.Parameters | ForEach-Object { New-ParameterXml $_ }) -join "`r`n"
)
  </ReportParameters>
"@
    } else { "" }

    $columnXml = ($spec.Columns | ForEach-Object { "                <TablixColumn><Width>$($_.Width)</Width></TablixColumn>" }) -join "`r`n"

    $headerCells = for ($i = 0; $i -lt $spec.Columns.Count; $i++) {
        $col = $spec.Columns[$i]
@"
                    <TablixCell>
                      <CellContents>
                        $(New-CellTextbox -name "Hdr$i" -value $col.Header.ToUpper() -align "Center" -bold $true -verticalAlign "Middle")
                      </CellContents>
                    </TablixCell>
"@
    }

    $detailCells = for ($i = 0; $i -lt $spec.Columns.Count; $i++) {
        $col = $spec.Columns[$i]
@"
                    <TablixCell>
                      <CellContents>
                        $(New-CellTextbox -name "Dtl$i" -value $col.Expression -align $col.Align -bold $false -verticalAlign "Top")
                      </CellContents>
                    </TablixCell>
"@
    }

    $tablixMembers = ($spec.Columns | ForEach-Object { "                <TablixMember />" }) -join "`r`n"

    $headerItems = @()
    $headerItems += (New-ParagraphTextbox -name "TxtTitle" -value $spec.Title -align "Center" -bold $true -fontSize "12pt" -verticalAlign "Middle" -top "0in" -left "0in" -width "7.45in" -height "0.24in")
    $headerItems += (New-ParagraphTextbox -name "TxtLeft1" -value $spec.HeaderLeft1 -bold $true -top "0.34in" -left "0in" -width "4.05in" -height "0.2in")
    $headerItems += (New-ParagraphTextbox -name "TxtRight1" -value $spec.HeaderRight1 -bold $true -top "0.34in" -left "4.15in" -width "3.3in" -height "0.2in")
    $headerItems += (New-ParagraphTextbox -name "TxtLeft2" -value $spec.HeaderLeft2 -bold $true -top "0.56in" -left "0in" -width "2in" -height "0.2in")
    $headerItems += (New-ParagraphTextbox -name "TxtLeft3" -value $spec.HeaderLeft3 -bold $true -top "0.56in" -left "2.05in" -width "2in" -height "0.2in")
    $headerItems += (New-ParagraphTextbox -name "TxtRight2" -value $spec.HeaderRight2 -bold $true -top "0.56in" -left "4.15in" -width "3.3in" -height "0.2in")
    $headerXml = ($headerItems -join "`r`n")

    $footerTitle = XmlEscape $spec.Title
    $footerPage = XmlEscape '="Page " & Globals!PageNumber & " of " & Globals!TotalPages'

    $reportXml = @"
<?xml version="1.0" encoding="utf-8"?>
<Report xmlns="http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition" xmlns:rd="http://schemas.microsoft.com/SQLServer/reporting/reportdesigner">
  <Body>
    <ReportItems>
      <Tablix Name="Tablix1">
        <TablixBody>
          <TablixColumns>
$columnXml
          </TablixColumns>
          <TablixRows>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
$($headerCells -join "`r`n")
              </TablixCells>
            </TablixRow>
            <TablixRow>
              <Height>0.25in</Height>
              <TablixCells>
$($detailCells -join "`r`n")
              </TablixCells>
            </TablixRow>
          </TablixRows>
        </TablixBody>
        <TablixColumnHierarchy>
          <TablixMembers>
$tablixMembers
          </TablixMembers>
        </TablixColumnHierarchy>
        <TablixRowHierarchy>
          <TablixMembers>
            <TablixMember>
              <KeepWithGroup>After</KeepWithGroup>
              <RepeatOnNewPage>true</RepeatOnNewPage>
            </TablixMember>
            <TablixMember>
              <Group Name="Details" />
            </TablixMember>
          </TablixMembers>
        </TablixRowHierarchy>
        <DataSetName>$($spec.DataSetName)</DataSetName>
        <Top>0in</Top>
        <Left>0in</Left>
        <Height>0.5in</Height>
        <Width>7.45in</Width>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Tablix>
    </ReportItems>
    <Height>0.5in</Height>
    <Style />
  </Body>
  <Width>7.45in</Width>
  <Page>
    <PageHeader>
      <Height>0.95in</Height>
      <PrintOnFirstPage>true</PrintOnFirstPage>
      <PrintOnLastPage>true</PrintOnLastPage>
      <ReportItems>
$headerXml
      </ReportItems>
      <Style />
    </PageHeader>
    <PageFooter>
      <Height>0.24in</Height>
      <PrintOnFirstPage>true</PrintOnFirstPage>
      <PrintOnLastPage>true</PrintOnLastPage>
      <ReportItems>
        $(New-ParagraphTextbox -name "FooterTitle" -value $footerTitle -bold $true -top "0in" -left "0in" -width "3in" -height "0.2in" -borderColor "White")
        $(New-ParagraphTextbox -name "FooterPage" -value $footerPage -align "Right" -bold $true -top "0in" -left "5.8in" -width "1.65in" -height "0.2in" -borderColor "White")
      </ReportItems>
      <Style />
    </PageFooter>
    <PageHeight>11.69in</PageHeight>
    <PageWidth>8.27in</PageWidth>
    <InteractiveHeight>11.69in</InteractiveHeight>
    <InteractiveWidth>8.27in</InteractiveWidth>
    <LeftMargin>0.25in</LeftMargin>
    <RightMargin>0.25in</RightMargin>
    <TopMargin>0.25in</TopMargin>
    <BottomMargin>0.25in</BottomMargin>
    <Style />
  </Page>
  <DataSources>
    <DataSource Name="$($spec.DataSourceName)">
      <ConnectionProperties>
        <DataProvider>System.Data.DataSet</DataProvider>
        <ConnectString>/* Local Connection */</ConnectString>
      </ConnectionProperties>
      <rd:DataSourceID>$(New-Guid)</rd:DataSourceID>
    </DataSource>
  </DataSources>
$parameterXml
  <DataSets>
    <DataSet Name="$($spec.DataSetName)">
      <Query>
        <DataSourceName>$($spec.DataSourceName)</DataSourceName>
        <CommandText>/* Local Query */</CommandText>
      </Query>
      <Fields>
$fieldXml
      </Fields>
    </DataSet>
  </DataSets>
  <Language>en-US</Language>
  <ConsumeContainerWhitespace>true</ConsumeContainerWhitespace>
  <rd:ReportUnitType>Inch</rd:ReportUnitType>
  <rd:ReportID>$(New-Guid)</rd:ReportID>
</Report>
"@

    Set-Content -LiteralPath $spec.Path -Value $reportXml -Encoding UTF8
}

$specs = @(
    @{
        Path = Join-Path $reportsDir 'Account_Pay.rdlc'
        Title = 'ACCOUNT PAYABLE'
        DataSourceName = 'LocalDataSource'
        DataSetName = 'DataSet1'
        Parameters = @('FromDate', 'ToDate')
        HeaderLeft1 = 'Payable Accounts'
        HeaderLeft2 = '= "From Date: " & Parameters!FromDate.Value'
        HeaderLeft3 = '= "To Date: " & Parameters!ToDate.Value'
        HeaderRight1 = '= "Printed On: " & Format(Globals!ExecutionTime, "dd-MMM-yyyy hh:mm tt")'
        HeaderRight2 = 'User:'
        Fields = @(
            @{ Name='AccountNo'; DataField='AccountNo'; TypeName='System.String' }
            @{ Name='AccountName'; DataField='AccountName'; TypeName='System.String' }
            @{ Name='Payable'; DataField='Payable'; TypeName='System.String' }
        )
        Columns = @(
            @{ Header='Account No'; Expression='=Fields!AccountNo.Value'; Width='1.5in'; Align='Left' }
            @{ Header='Account Name'; Expression='=Fields!AccountName.Value'; Width='4.25in'; Align='Left' }
            @{ Header='Payable'; Expression='=Fields!Payable.Value'; Width='1.7in'; Align='Right' }
        )
    },
    @{
        Path = Join-Path $reportsDir 'Account_Receivables.rdlc'
        Title = 'ACCOUNT RECEIVABLES'
        DataSourceName = 'LocalDataSource'
        DataSetName = 'DataSet1'
        Parameters = @()
        HeaderLeft1 = 'Receivable Accounts'
        HeaderLeft2 = '= ""'
        HeaderLeft3 = '= ""'
        HeaderRight1 = '= "Printed On: " & Format(Globals!ExecutionTime, "dd-MMM-yyyy hh:mm tt")'
        HeaderRight2 = 'User:'
        Fields = @(
            @{ Name='ACCOUNTNO'; DataField='ACCOUNTNO'; TypeName='System.String' }
            @{ Name='ACCOUNTNAME'; DataField='ACCOUNTNAME'; TypeName='System.String' }
            @{ Name='CLOSINGBAL'; DataField='CLOSINGBAL'; TypeName='System.String' }
            @{ Name='CLOSINGBALTYPE'; DataField='CLOSINGBALTYPE'; TypeName='System.String' }
        )
        Columns = @(
            @{ Header='Account No'; Expression='=Fields!ACCOUNTNO.Value'; Width='1.5in'; Align='Left' }
            @{ Header='Account Name'; Expression='=Fields!ACCOUNTNAME.Value'; Width='4.15in'; Align='Left' }
            @{ Header='Closing Balance'; Expression='=Fields!CLOSINGBAL.Value & " " & Fields!CLOSINGBALTYPE.Value'; Width='1.8in'; Align='Right' }
        )
    },
    @{
        Path = Join-Path $reportsDir 'Accounts_Ledger.rdlc'
        Title = 'ACCOUNTS LEDGER'
        DataSourceName = 'LocalDataSource'
        DataSetName = 'DataSet1'
        Parameters = @('PartyName', 'PartyID')
        HeaderLeft1 = '= "Party Name: " & Parameters!PartyName.Value'
        HeaderLeft2 = '= "Party ID: " & Parameters!PartyID.Value'
        HeaderLeft3 = '= ""'
        HeaderRight1 = '= "Printed On: " & Format(Globals!ExecutionTime, "dd-MMM-yyyy hh:mm tt")'
        HeaderRight2 = 'User:'
        Fields = @(
            @{ Name='VoucherNo'; DataField='VoucherNo'; TypeName='System.String' }
            @{ Name='VoucherType'; DataField='VoucherType'; TypeName='System.String' }
            @{ Name='VoucherDate'; DataField='VoucherDate'; TypeName='System.String' }
            @{ Name='Credit'; DataField='Credit'; TypeName='System.String' }
            @{ Name='Debit'; DataField='Debit'; TypeName='System.String' }
            @{ Name='Narration'; DataField='Narration'; TypeName='System.String' }
            @{ Name='DisplayDate'; DataField='DisplayDate'; TypeName='System.String' }
            @{ Name='Balance'; DataField='Balance'; TypeName='System.String' }
            @{ Name='BalanceType'; DataField='BalanceType'; TypeName='System.String' }
            @{ Name='Voucher'; DataField='Voucher'; TypeName='System.String' }
        )
        Columns = @(
            @{ Header='Date'; Expression='=Fields!DisplayDate.Value'; Width='0.9in'; Align='Left' }
            @{ Header='Voucher'; Expression='=Fields!VoucherNo.Value'; Width='0.95in'; Align='Left' }
            @{ Header='Type'; Expression='=Fields!VoucherType.Value'; Width='0.8in'; Align='Left' }
            @{ Header='Description'; Expression='=Fields!Narration.Value'; Width='2.95in'; Align='Left' }
            @{ Header='Debit'; Expression='=Fields!Debit.Value'; Width='0.85in'; Align='Right' }
            @{ Header='Credit'; Expression='=Fields!Credit.Value'; Width='0.85in'; Align='Right' }
            @{ Header='Balance'; Expression='=Fields!Balance.Value & " " & Fields!BalanceType.Value'; Width='1.15in'; Align='Right' }
        )
    },
    @{
        Path = Join-Path $reportsDir 'Daily_Parties_ledger.rdlc'
        Title = 'DAILY PARTIES LEDGER'
        DataSourceName = 'LocalDataSource'
        DataSetName = 'DataSet1'
        Parameters = @()
        HeaderLeft1 = '= "Party: " & IIF(CountDistinct(Fields!OrderCol.Value, "DataSet1")=1, First(Fields!OrderCol.Value, "DataSet1"), "")'
        HeaderLeft2 = '= "From Date: " & IIF(CountRows("DataSet1")=0, "", Format(Min(Fields!VoucherDate.Value, "DataSet1"), "dd-MMM-yyyy"))'
        HeaderLeft3 = '= "To Date: " & IIF(CountRows("DataSet1")=0, "", Format(Max(Fields!VoucherDate.Value, "DataSet1"), "dd-MMM-yyyy"))'
        HeaderRight1 = '= "Printed On: " & Format(Globals!ExecutionTime, "dd-MMM-yyyy hh:mm tt")'
        HeaderRight2 = 'User:'
        Fields = @(
            @{ Name='VoucherNo'; DataField='VoucherNo'; TypeName='System.String' }
            @{ Name='VoucherDate'; DataField='VoucherDate'; TypeName='System.DateTime' }
            @{ Name='Debit'; DataField='Debit'; TypeName='System.String' }
            @{ Name='Credit'; DataField='Credit'; TypeName='System.String' }
            @{ Name='Shipment'; DataField='Shipment'; TypeName='System.String' }
            @{ Name='OrderCol'; DataField='OrderCol'; TypeName='System.String' }
            @{ Name='Refrence'; DataField='Refrence'; TypeName='System.String' }
        )
        Columns = @(
            @{ Header='Date'; Expression='=Format(Fields!VoucherDate.Value, "dd-MMM-yyyy")'; Width='0.95in'; Align='Left' }
            @{ Header='Voucher No'; Expression='=Fields!VoucherNo.Value'; Width='1.0in'; Align='Left' }
            @{ Header='Reference'; Expression='=Fields!Refrence.Value'; Width='1.0in'; Align='Left' }
            @{ Header='Description'; Expression='=Fields!Shipment.Value'; Width='2.75in'; Align='Left' }
            @{ Header='Debit'; Expression='=Fields!Debit.Value'; Width='0.85in'; Align='Right' }
            @{ Header='Credit'; Expression='=Fields!Credit.Value'; Width='0.85in'; Align='Right' }
        )
    },
    @{
        Path = Join-Path $reportsDir 'DayBook.rdlc'
        Title = 'DAY BOOK'
        DataSourceName = 'LocalDataSource'
        DataSetName = 'DayBookDataSet'
        Parameters = @('FromDate', 'ToDate')
        HeaderLeft1 = 'Day Book Register'
        HeaderLeft2 = '= "From Date: " & Parameters!FromDate.Value'
        HeaderLeft3 = '= "To Date: " & Parameters!ToDate.Value'
        HeaderRight1 = '= "Printed On: " & Format(Globals!ExecutionTime, "dd-MMM-yyyy hh:mm tt")'
        HeaderRight2 = 'User:'
        Fields = @(
            @{ Name='VoucherDate'; DataField='VoucherDate'; TypeName='System.DateTime' }
            @{ Name='VoucherNo'; DataField='VoucherNo'; TypeName='System.String' }
            @{ Name='VoucherType'; DataField='VoucherType'; TypeName='System.String' }
            @{ Name='AccountNo'; DataField='AccountNo'; TypeName='System.String' }
            @{ Name='Debit'; DataField='Debit'; TypeName='System.Decimal' }
            @{ Name='Credit'; DataField='Credit'; TypeName='System.Decimal' }
            @{ Name='Narration'; DataField='Narration'; TypeName='System.String' }
        )
        Columns = @(
            @{ Header='Date'; Expression='=Format(Fields!VoucherDate.Value, "dd-MMM-yyyy")'; Width='0.95in'; Align='Left' }
            @{ Header='Voucher No'; Expression='=Fields!VoucherNo.Value'; Width='0.95in'; Align='Left' }
            @{ Header='Type'; Expression='=Fields!VoucherType.Value'; Width='0.8in'; Align='Left' }
            @{ Header='Account'; Expression='=Fields!AccountNo.Value'; Width='1.15in'; Align='Left' }
            @{ Header='Description'; Expression='=Fields!Narration.Value'; Width='2.2in'; Align='Left' }
            @{ Header='Debit'; Expression='=IIF(Fields!Debit.Value = 0, "", Format(Fields!Debit.Value, "N2"))'; Width='0.7in'; Align='Right' }
            @{ Header='Credit'; Expression='=IIF(Fields!Credit.Value = 0, "", Format(Fields!Credit.Value, "N2"))'; Width='0.7in'; Align='Right' }
        )
    },
    @{
        Path = Join-Path $reportsDir 'Party_Detailed_Ledger_Combine.rdlc'
        Title = '=Parameters!ReportTitle.Value'
        DataSourceName = 'LocalDataSource'
        DataSetName = 'DataSet1'
        Parameters = @('ReportTitle','FromDate','ToDate','AccountName','PrintedOn','PrintedBy')
        HeaderLeft1 = '= "Account: " & Parameters!AccountName.Value'
        HeaderLeft2 = '= "From Date: " & Parameters!FromDate.Value'
        HeaderLeft3 = '= "To Date: " & Parameters!ToDate.Value'
        HeaderRight1 = '= "Printed On: " & Parameters!PrintedOn.Value'
        HeaderRight2 = '= "User: " & Parameters!PrintedBy.Value'
        Fields = @(
            @{ Name='RowType'; DataField='RowType'; TypeName='System.String' }
            @{ Name='VoucherDate'; DataField='VoucherDate'; TypeName='System.DateTime' }
            @{ Name='DisplayDate'; DataField='DisplayDate'; TypeName='System.String' }
            @{ Name='VoucherNo'; DataField='VoucherNo'; TypeName='System.String' }
            @{ Name='DisplayVoucherNo'; DataField='DisplayVoucherNo'; TypeName='System.String' }
            @{ Name='Refrence'; DataField='Refrence'; TypeName='System.String' }
            @{ Name='DisplayReference'; DataField='DisplayReference'; TypeName='System.String' }
            @{ Name='BilltyNo'; DataField='BilltyNo'; TypeName='System.String' }
            @{ Name='Shipment'; DataField='Shipment'; TypeName='System.String' }
            @{ Name='DisplayDocProof'; DataField='DisplayDocProof'; TypeName='System.String' }
            @{ Name='Narration'; DataField='Narration'; TypeName='System.String' }
            @{ Name='DisplayDescription'; DataField='DisplayDescription'; TypeName='System.String' }
            @{ Name='Debit'; DataField='Debit'; TypeName='System.Decimal' }
            @{ Name='Credit'; DataField='Credit'; TypeName='System.Decimal' }
            @{ Name='DebitText'; DataField='DebitText'; TypeName='System.String' }
            @{ Name='CreditText'; DataField='CreditText'; TypeName='System.String' }
            @{ Name='Balance'; DataField='Balance'; TypeName='System.Decimal' }
            @{ Name='BalanceText'; DataField='BalanceText'; TypeName='System.String' }
        )
        Columns = @(
            @{ Header='Date'; Expression='=Fields!DisplayDate.Value'; Width='0.85in'; Align='Left' }
            @{ Header='Voucher'; Expression='=Fields!DisplayVoucherNo.Value'; Width='0.95in'; Align='Left' }
            @{ Header='Reference'; Expression='=Fields!DisplayReference.Value'; Width='0.95in'; Align='Left' }
            @{ Header='Doc'; Expression='=Fields!DisplayDocProof.Value'; Width='0.8in'; Align='Left' }
            @{ Header='Description'; Expression='=Fields!DisplayDescription.Value'; Width='2.35in'; Align='Left' }
            @{ Header='Debit'; Expression='=Fields!DebitText.Value'; Width='0.75in'; Align='Right' }
            @{ Header='Credit'; Expression='=Fields!CreditText.Value'; Width='0.75in'; Align='Right' }
            @{ Header='Balance'; Expression='=Fields!BalanceText.Value'; Width='1.05in'; Align='Right' }
        )
    },
    @{
        Path = Join-Path $reportsDir 'PartyLedgerDetailed.rdlc'
        Title = '=Parameters!ReportTitle.Value'
        DataSourceName = 'LocalDataSource'
        DataSetName = 'LedgerDataSet'
        Parameters = @('ReportTitle','FromDate','ToDate','AccountName','PrintedOn','PrintedBy')
        HeaderLeft1 = '= "Account: " & Parameters!AccountName.Value'
        HeaderLeft2 = '= "From Date: " & Parameters!FromDate.Value'
        HeaderLeft3 = '= "To Date: " & Parameters!ToDate.Value'
        HeaderRight1 = '= "Printed On: " & Parameters!PrintedOn.Value'
        HeaderRight2 = '= "User: " & Parameters!PrintedBy.Value'
        Fields = @(
            @{ Name='RowType'; DataField='RowType'; TypeName='System.String' }
            @{ Name='LineNo'; DataField='LineNo'; TypeName='System.Int32' }
            @{ Name='VoucherDate'; DataField='VoucherDate'; TypeName='System.DateTime' }
            @{ Name='DisplayDate'; DataField='DisplayDate'; TypeName='System.String' }
            @{ Name='VoucherNo'; DataField='VoucherNo'; TypeName='System.String' }
            @{ Name='Reference'; DataField='Reference'; TypeName='System.String' }
            @{ Name='DocProof'; DataField='DocProof'; TypeName='System.String' }
            @{ Name='Description'; DataField='Description'; TypeName='System.String' }
            @{ Name='DebitAmount'; DataField='DebitAmount'; TypeName='System.Decimal' }
            @{ Name='CreditAmount'; DataField='CreditAmount'; TypeName='System.Decimal' }
            @{ Name='BalanceAmount'; DataField='BalanceAmount'; TypeName='System.Decimal' }
            @{ Name='BalanceNature'; DataField='BalanceNature'; TypeName='System.String' }
            @{ Name='BalanceText'; DataField='BalanceText'; TypeName='System.String' }
        )
        Columns = @(
            @{ Header='Date'; Expression='=Fields!DisplayDate.Value'; Width='0.85in'; Align='Left' }
            @{ Header='Voucher'; Expression='=Fields!VoucherNo.Value'; Width='0.95in'; Align='Left' }
            @{ Header='Reference'; Expression='=Fields!Reference.Value'; Width='0.95in'; Align='Left' }
            @{ Header='Doc'; Expression='=Fields!DocProof.Value'; Width='0.8in'; Align='Left' }
            @{ Header='Description'; Expression='=Fields!Description.Value'; Width='2.35in'; Align='Left' }
            @{ Header='Debit'; Expression='=IIF(Fields!DebitAmount.Value = 0, "", Format(Fields!DebitAmount.Value, "N2"))'; Width='0.75in'; Align='Right' }
            @{ Header='Credit'; Expression='=IIF(Fields!CreditAmount.Value = 0, "", Format(Fields!CreditAmount.Value, "N2"))'; Width='0.75in'; Align='Right' }
            @{ Header='Balance'; Expression='=Fields!BalanceText.Value'; Width='1.05in'; Align='Right' }
        )
    },
    @{
        Path = Join-Path $reportsDir 'Product_Price_List.rdlc'
        Title = 'PRODUCT PRICE LIST'
        DataSourceName = 'LocalDataSource'
        DataSetName = 'DataSet1'
        Parameters = @('CompanyName','PriceType')
        HeaderLeft1 = '= "Company: " & Parameters!CompanyName.Value'
        HeaderLeft2 = '= "Price Type: " & Parameters!PriceType.Value'
        HeaderLeft3 = '= ""'
        HeaderRight1 = '= "Printed On: " & Format(Globals!ExecutionTime, "dd-MMM-yyyy hh:mm tt")'
        HeaderRight2 = 'User:'
        Fields = @(
            @{ Name='ProductID'; DataField='ProductID'; TypeName='System.String' }
            @{ Name='ProductName'; DataField='ProductName'; TypeName='System.String' }
            @{ Name='RetailPrice'; DataField='RetailPrice'; TypeName='System.String' }
            @{ Name='PurchasePrice'; DataField='PurchasePrice'; TypeName='System.String' }
            @{ Name='PurDiscRatio'; DataField='PurDiscRatio'; TypeName='System.String' }
            @{ Name='PurDiscValue'; DataField='PurDiscValue'; TypeName='System.String' }
            @{ Name='PurSTRatio'; DataField='PurSTRatio'; TypeName='System.String' }
            @{ Name='SalePrice'; DataField='SalePrice'; TypeName='System.String' }
            @{ Name='SaleDiscRatio'; DataField='SaleDiscRatio'; TypeName='System.String' }
            @{ Name='SaleDiscValue'; DataField='SaleDiscValue'; TypeName='System.String' }
            @{ Name='SaleSTRatio'; DataField='SaleSTRatio'; TypeName='System.String' }
        )
        Columns = @(
            @{ Header='Product'; Expression='=Fields!ProductName.Value'; Width='3.85in'; Align='Left' }
            @{ Header='Purchase Price'; Expression='=Fields!PurchasePrice.Value'; Width='1.2in'; Align='Right' }
            @{ Header='Retail Price'; Expression='=Fields!RetailPrice.Value'; Width='1.2in'; Align='Right' }
            @{ Header='Sale Price'; Expression='=Fields!SalePrice.Value'; Width='1.2in'; Align='Right' }
        )
    },
    @{
        Path = Join-Path $reportsDir 'Purchase_Return_Summary.rdlc'
        Title = 'PURCHASE RETURN SUMMARY'
        DataSourceName = 'LocalDataSource'
        DataSetName = 'DataSet1'
        Parameters = @('FromDate','ToDate')
        HeaderLeft1 = 'Purchase Returns'
        HeaderLeft2 = '= "From Date: " & Parameters!FromDate.Value'
        HeaderLeft3 = '= "To Date: " & Parameters!ToDate.Value'
        HeaderRight1 = '= "Printed On: " & Format(Globals!ExecutionTime, "dd-MMM-yyyy hh:mm tt")'
        HeaderRight2 = 'User:'
        Fields = @(
            @{ Name='PurReturnId'; DataField='PurReturnId'; TypeName='System.String' }
            @{ Name='PurchaseId'; DataField='PurchaseId'; TypeName='System.String' }
            @{ Name='ReturnDate'; DataField='ReturnDate'; TypeName='System.String' }
            @{ Name='VendorName'; DataField='VendorName'; TypeName='System.String' }
            @{ Name='GrossValue'; DataField='GrossValue'; TypeName='System.String' }
            @{ Name='Discount'; DataField='Discount'; TypeName='System.String' }
            @{ Name='SalesTax'; DataField='SalesTax'; TypeName='System.String' }
            @{ Name='NetValue'; DataField='NetValue'; TypeName='System.String' }
        )
        Columns = @(
            @{ Header='Date'; Expression='=Fields!ReturnDate.Value'; Width='0.95in'; Align='Left' }
            @{ Header='Invoice'; Expression='=Fields!PurchaseId.Value'; Width='1.0in'; Align='Left' }
            @{ Header='Vendor'; Expression='=Fields!VendorName.Value'; Width='2.45in'; Align='Left' }
            @{ Header='Gross'; Expression='=Fields!GrossValue.Value'; Width='0.9in'; Align='Right' }
            @{ Header='Discount'; Expression='=Fields!Discount.Value'; Width='0.8in'; Align='Right' }
            @{ Header='Tax'; Expression='=Fields!SalesTax.Value'; Width='0.7in'; Align='Right' }
            @{ Header='Net'; Expression='=Fields!NetValue.Value'; Width='0.85in'; Align='Right' }
        )
    },
    @{
        Path = Join-Path $reportsDir 'Sale_Return_Summary.rdlc'
        Title = 'SALE RETURN SUMMARY'
        DataSourceName = 'LocalDataSource'
        DataSetName = 'DataSet1'
        Parameters = @('FromDate','ToDate')
        HeaderLeft1 = 'Sale Returns'
        HeaderLeft2 = '= "From Date: " & Parameters!FromDate.Value'
        HeaderLeft3 = '= "To Date: " & Parameters!ToDate.Value'
        HeaderRight1 = '= "Printed On: " & Format(Globals!ExecutionTime, "dd-MMM-yyyy hh:mm tt")'
        HeaderRight2 = 'User:'
        Fields = @(
            @{ Name='ReturnId'; DataField='ReturnId'; TypeName='System.String' }
            @{ Name='ReturnDate'; DataField='ReturnDate'; TypeName='System.String' }
            @{ Name='SaleId'; DataField='SaleId'; TypeName='System.String' }
            @{ Name='CustomerName'; DataField='CustomerName'; TypeName='System.String' }
            @{ Name='GrossValue'; DataField='GrossValue'; TypeName='System.String' }
            @{ Name='Discount'; DataField='Discount'; TypeName='System.String' }
            @{ Name='Sales'; DataField='Sales'; TypeName='System.String' }
            @{ Name='NetValue'; DataField='NetValue'; TypeName='System.String' }
        )
        Columns = @(
            @{ Header='Date'; Expression='=Fields!ReturnDate.Value'; Width='0.95in'; Align='Left' }
            @{ Header='Invoice'; Expression='=Fields!SaleId.Value'; Width='1.0in'; Align='Left' }
            @{ Header='Customer'; Expression='=Fields!CustomerName.Value'; Width='2.45in'; Align='Left' }
            @{ Header='Gross'; Expression='=Fields!GrossValue.Value'; Width='0.9in'; Align='Right' }
            @{ Header='Discount'; Expression='=Fields!Discount.Value'; Width='0.8in'; Align='Right' }
            @{ Header='Sales'; Expression='=Fields!Sales.Value'; Width='0.7in'; Align='Right' }
            @{ Header='Net'; Expression='=Fields!NetValue.Value'; Width='0.85in'; Align='Right' }
        )
    }
)

foreach ($spec in $specs) {
    New-ClassicReport $spec
    Write-Host "Updated $($spec.Path)"
}
