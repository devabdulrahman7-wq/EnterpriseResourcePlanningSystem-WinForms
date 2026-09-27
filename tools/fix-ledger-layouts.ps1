$ErrorActionPreference = 'Stop'

function New-TextRunXml {
    param(
        [string]$Value,
        [bool]$Bold = $false,
        [string]$FontSize = '8pt'
    )

    $escapedValue = [System.Security.SecurityElement]::Escape($Value)

@"
<TextRun>
  <Value>$escapedValue</Value>
  <Style>
    <FontFamily>Arial</FontFamily>
    <FontSize>$FontSize</FontSize>
$(if ($Bold) { '    <FontWeight>Bold</FontWeight>' })
  </Style>
</TextRun>
"@
}

function New-TextboxXml {
    param(
        [string]$Name,
        [string]$Value,
        [string]$Align = 'Left',
        [bool]$Bold = $false,
        [string]$FontSize = '8pt',
        [string]$VerticalAlign = 'Top'
    )

@"
<Textbox xmlns="http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition" Name="$Name">
  <CanGrow>true</CanGrow>
  <KeepTogether>true</KeepTogether>
  <Paragraphs>
    <Paragraph>
      <TextRuns>
$(New-TextRunXml -Value $Value -Bold:$Bold -FontSize $FontSize)
      </TextRuns>
      <Style>
        <TextAlign>$Align</TextAlign>
      </Style>
    </Paragraph>
  </Paragraphs>
  <Style>
    <Border>
      <Color>Black</Color>
      <Style>Solid</Style>
    </Border>
    <VerticalAlign>$VerticalAlign</VerticalAlign>
    <PaddingLeft>2pt</PaddingLeft>
    <PaddingRight>2pt</PaddingRight>
    <PaddingTop>2pt</PaddingTop>
    <PaddingBottom>2pt</PaddingBottom>
  </Style>
</Textbox>
"@
}

function New-TablixCellXml {
    param([string]$InnerTextboxXml)

@"
<TablixCell xmlns="http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition">
  <CellContents>
$InnerTextboxXml
  </CellContents>
</TablixCell>
"@
}

function New-TablixRowXml {
    param(
        [double]$HeightIn,
        [string[]]$CellXml
    )

@"
<TablixRow xmlns="http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition">
  <Height>$("{0:0.##}in" -f $HeightIn)</Height>
  <TablixCells>
$($CellXml -join "`r`n")
  </TablixCells>
</TablixRow>
"@
}

function Build-TablixXml {
    param(
        [string]$DataSetName,
        [array]$Columns,
        [array]$FooterRows
    )

    $tablixColumns = @()
    $headerCells = @()
    $detailCells = @()
    $footerRowXml = @()

    for ($i = 0; $i -lt $Columns.Count; $i++) {
        $col = $Columns[$i]
        $tablixColumns += "<TablixColumn xmlns=`"http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition`"><Width>$($col.Width)</Width></TablixColumn>"
        $headerCells += New-TablixCellXml (New-TextboxXml -Name "Hdr$i" -Value $col.Header -Align 'Center' -Bold $true -VerticalAlign 'Middle')
        $detailCells += New-TablixCellXml (New-TextboxXml -Name "Dtl$i" -Value $col.Detail -Align $col.Align)
    }

    if ($FooterRows) {
        for ($r = 0; $r -lt $FooterRows.Count; $r++) {
            $cells = @()
            for ($i = 0; $i -lt $Columns.Count; $i++) {
                $value = if ($FooterRows[$r].Cells.Count -gt $i) { $FooterRows[$r].Cells[$i] } else { '=""' }
                $align = if ($FooterRows[$r].Align.Count -gt $i) { $FooterRows[$r].Align[$i] } else { 'Left' }
                $cells += New-TablixCellXml (New-TextboxXml -Name "Ftr${r}_$i" -Value $value -Align $align -Bold $true)
            }
            $footerRowXml += New-TablixRowXml -HeightIn 0.25 -CellXml $cells
        }
    }

    $rowMembers = @(
        '<TablixMember xmlns="http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition"><KeepWithGroup>After</KeepWithGroup><RepeatOnNewPage>true</RepeatOnNewPage></TablixMember>',
        '<TablixMember xmlns="http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition"><Group Name="Details" /></TablixMember>'
    )

    foreach ($null in $FooterRows) {
        $rowMembers += '<TablixMember xmlns="http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition"><KeepWithGroup>Before</KeepWithGroup></TablixMember>'
    }

    $widthTotal = 0.0
    foreach ($col in $Columns) {
        $widthTotal += [double]($col.Width -replace 'in','')
    }

@"
<Tablix xmlns="http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition" Name="Tablix1">
  <TablixBody>
    <TablixColumns>
$($tablixColumns -join "`r`n")
    </TablixColumns>
    <TablixRows>
$(New-TablixRowXml -HeightIn 0.25 -CellXml $headerCells)
$(New-TablixRowXml -HeightIn 0.25 -CellXml $detailCells)
$($footerRowXml -join "`r`n")
    </TablixRows>
  </TablixBody>
  <TablixColumnHierarchy>
    <TablixMembers>
$((1..$Columns.Count | ForEach-Object { '      <TablixMember xmlns="http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition" />' }) -join "`r`n")
    </TablixMembers>
  </TablixColumnHierarchy>
  <TablixRowHierarchy>
    <TablixMembers>
$($rowMembers -join "`r`n")
    </TablixMembers>
  </TablixRowHierarchy>
  <DataSetName>$DataSetName</DataSetName>
  <Top>0in</Top>
  <Left>0in</Left>
  <Height>$("{0:0.##}in" -f (0.25 * (2 + $FooterRows.Count)))</Height>
  <Width>$("{0:0.##}in" -f $widthTotal)</Width>
  <Style>
    <Border>
      <Style>None</Style>
    </Border>
  </Style>
</Tablix>
"@
}

function Replace-BodyWithTablix {
    param(
        [string]$Path,
        [string]$TablixXml
    )

    $content = Get-Content -LiteralPath $Path -Raw
    [xml]$xml = $content
    $ns = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
    $ns.AddNamespace('def', 'http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition')

    $body = $xml.SelectSingleNode('/def:Report/def:Body', $ns)
    $reportItems = $body.SelectSingleNode('def:ReportItems', $ns)
    if (-not $reportItems) {
        $reportItems = $xml.CreateElement('ReportItems', $ns.LookupNamespace('def'))
        [void]$body.AppendChild($reportItems)
    }

    while ($reportItems.HasChildNodes) {
        [void]$reportItems.RemoveChild($reportItems.FirstChild)
    }

    $fragment = $xml.CreateDocumentFragment()
    $fragment.InnerXml = $TablixXml
    [void]$reportItems.AppendChild($fragment.FirstChild)

    $bodyHeight = $body.SelectSingleNode('def:Height', $ns)
    if (-not $bodyHeight) {
        $bodyHeight = $xml.CreateElement('Height', $ns.LookupNamespace('def'))
        [void]$body.AppendChild($bodyHeight)
    }
    $bodyHeight.InnerText = '1in'

    $widthNode = $xml.SelectSingleNode('/def:Report/def:Width', $ns)
    if ($widthNode) {
        $widthNode.InnerText = '7.45in'
    }

    $settings = New-Object System.Xml.XmlWriterSettings
    $settings.Indent = $true
    $settings.IndentChars = '  '
    $settings.NewLineChars = "`r`n"
    $settings.NewLineHandling = 'Replace'
    $settings.OmitXmlDeclaration = $false

    $writer = [System.Xml.XmlWriter]::Create($Path, $settings)
    $xml.Save($writer)
    $writer.Dispose()
}

function N2Expr([string]$fieldName, [string]$dataSetName) {
    return "=IIF(IsNothing(Fields!$fieldName.Value), `"`", Format(Fields!$fieldName.Value, `"N2`"))"
}

function SumExpr([string]$fieldName, [string]$dataSetName) {
    return "=IIF(CountRows(`"$dataSetName`") = 0, `"`", Format(Sum(Fields!$fieldName.Value, `"$dataSetName`"), `"N2`"))"
}

function LastExpr([string]$fieldName, [string]$dataSetName) {
    return "=IIF(CountRows(`"$dataSetName`") = 0, `"`", Format(Last(Fields!$fieldName.Value, `"$dataSetName`"), `"N2`"))"
}

function TextOrBlankExpr([string]$fieldName) {
    return "=IIF(IsNothing(Fields!$fieldName.Value), `"`", Fields!$fieldName.Value)"
}

$targets = @(
    @{
        Path = 'FO-2022\Reports\Accounts_Ledger.rdlc'
        DataSet = 'DataSet1'
        Columns = @(
            @{ Header='DATE'; Width='0.80in'; Detail='=IIF(IsNothing(Fields!VoucherDate.Value), "", Format(Fields!VoucherDate.Value, "dd-MMM-yyyy"))'; Align='Left' },
            @{ Header='DOC NO'; Width='0.85in'; Detail='=Fields!VoucherNo.Value'; Align='Left' },
            @{ Header='REFERENCE'; Width='1.00in'; Detail='=Fields!VoucherType.Value'; Align='Left' },
            @{ Header='DESCRIPTION'; Width='2.40in'; Detail='=Fields!Narration.Value'; Align='Left' },
            @{ Header='DEBIT'; Width='0.80in'; Detail=(N2Expr 'Debit' 'DataSet1'); Align='Right' },
            @{ Header='CREDIT'; Width='0.80in'; Detail=(N2Expr 'Credit' 'DataSet1'); Align='Right' },
            @{ Header='BALANCE'; Width='0.80in'; Detail=(N2Expr 'Balance' 'DataSet1'); Align='Right' }
        )
        FooterRows = @(
            @{ Cells=@('="TOTAL"','=""','=""','=""',(SumExpr 'Debit' 'DataSet1'),(SumExpr 'Credit' 'DataSet1'),'=""'); Align=@('Left','Left','Left','Left','Right','Right','Right') },
            @{ Cells=@('="CLOSING BALANCE"','=""','=""','=""','=""','""',(LastExpr 'Balance' 'DataSet1')); Align=@('Left','Left','Left','Left','Right','Right','Right') }
        )
    },
    @{
        Path = 'FO-2022\Reports\Accounts_Ledger_1.rdlc'
        DataSet = 'DataSet1'
        Columns = @(
            @{ Header='DATE'; Width='0.80in'; Detail='=IIF(IsNothing(Fields!VoucherDate.Value), "", Format(Fields!VoucherDate.Value, "dd-MMM-yyyy"))'; Align='Left' },
            @{ Header='DOC NO'; Width='0.85in'; Detail='=Fields!VoucherNo.Value'; Align='Left' },
            @{ Header='REFERENCE'; Width='1.00in'; Detail='=Fields!ReferenceNo.Value'; Align='Left' },
            @{ Header='DESCRIPTION'; Width='2.40in'; Detail='=IIF(Len(Trim(CStr(Fields!Description.Value))) > 0, Fields!Description.Value, Fields!Narration.Value)'; Align='Left' },
            @{ Header='DEBIT'; Width='0.80in'; Detail=(N2Expr 'Debit' 'DataSet1'); Align='Right' },
            @{ Header='CREDIT'; Width='0.80in'; Detail=(N2Expr 'Credit' 'DataSet1'); Align='Right' },
            @{ Header='BALANCE'; Width='0.80in'; Detail=(N2Expr 'Balance' 'DataSet1'); Align='Right' }
        )
        FooterRows = @(
            @{ Cells=@('="TOTAL"','=""','=""','=""',(SumExpr 'Debit' 'DataSet1'),(SumExpr 'Credit' 'DataSet1'),'=""'); Align=@('Left','Left','Left','Left','Right','Right','Right') },
            @{ Cells=@('="CLOSING BALANCE"','=""','=""','=""','=""','""',(LastExpr 'Balance' 'DataSet1')); Align=@('Left','Left','Left','Left','Right','Right','Right') }
        )
    },
    @{
        Path = 'FO-2022\Reports\Bank_Statement.rdlc'
        DataSet = 'DataSet1'
        Columns = @(
            @{ Header='DATE'; Width='0.80in'; Detail='=IIF(IsNothing(Fields!TransactionDate.Value), "", Format(Fields!TransactionDate.Value, "dd-MMM-yyyy"))'; Align='Left' },
            @{ Header='DOC NO'; Width='0.85in'; Detail='=Fields!VoucherNo.Value'; Align='Left' },
            @{ Header='REFERENCE'; Width='1.00in'; Detail='=Fields!VoucherTypeCode.Value'; Align='Left' },
            @{ Header='DESCRIPTION'; Width='2.40in'; Detail='=Fields!Description.Value'; Align='Left' },
            @{ Header='DEBIT'; Width='0.80in'; Detail=(N2Expr 'Debit' 'DataSet1'); Align='Right' },
            @{ Header='CREDIT'; Width='0.80in'; Detail=(N2Expr 'Credit' 'DataSet1'); Align='Right' },
            @{ Header='BALANCE'; Width='0.80in'; Detail=(N2Expr 'Balance' 'DataSet1'); Align='Right' }
        )
        FooterRows = @(
            @{ Cells=@('="TOTAL"','=""','=""','=""',(SumExpr 'Debit' 'DataSet1'),(SumExpr 'Credit' 'DataSet1'),'=""'); Align=@('Left','Left','Left','Left','Right','Right','Right') },
            @{ Cells=@('="CLOSING BALANCE"','=""','=""','=""','=""','""',(LastExpr 'Balance' 'DataSet1')); Align=@('Left','Left','Left','Left','Right','Right','Right') }
        )
    },
    @{
        Path = 'FO-2022\Reports\Cash_Book.rdlc'
        DataSet = 'DataSet1'
        Columns = @(
            @{ Header='DATE'; Width='0.80in'; Detail='=IIF(IsNothing(Fields!VoucherDate.Value), "", Format(Fields!VoucherDate.Value, "dd-MMM-yyyy"))'; Align='Left' },
            @{ Header='DOC NO'; Width='0.85in'; Detail='=Fields!StrVoucherNo.Value'; Align='Left' },
            @{ Header='REFERENCE'; Width='1.00in'; Detail='=Fields!VoucherType.Value'; Align='Left' },
            @{ Header='DESCRIPTION'; Width='2.40in'; Detail='=Fields!Narration.Value'; Align='Left' },
            @{ Header='DEBIT'; Width='0.80in'; Detail=(N2Expr 'Debit' 'DataSet1'); Align='Right' },
            @{ Header='CREDIT'; Width='0.80in'; Detail=(N2Expr 'Credit' 'DataSet1'); Align='Right' },
            @{ Header='BALANCE'; Width='0.80in'; Detail='=""'; Align='Right' }
        )
        FooterRows = @(
            @{ Cells=@('="TOTAL"','=""','=""','=""',(SumExpr 'Debit' 'DataSet1'),(SumExpr 'Credit' 'DataSet1'),'""'); Align=@('Left','Left','Left','Left','Right','Right','Right') }
        )
    },
    @{
        Path = 'FO-2022\Reports\Daily_Parties_ledger.rdlc'
        DataSet = 'DataSet1'
        Columns = @(
            @{ Header='DATE'; Width='0.80in'; Detail='=IIF(IsNothing(Fields!VoucherDate.Value), "", Format(Fields!VoucherDate.Value, "dd-MMM-yyyy"))'; Align='Left' },
            @{ Header='DOC NO'; Width='0.85in'; Detail='=Fields!VoucherNo.Value'; Align='Left' },
            @{ Header='REFERENCE'; Width='1.00in'; Detail='=Fields!Refrence.Value'; Align='Left' },
            @{ Header='DESCRIPTION'; Width='2.40in'; Detail='=IIF(Len(Trim(CStr(Fields!Shipment.Value))) > 0, Fields!Shipment.Value & " / " & Fields!OrderCol.Value, Fields!OrderCol.Value)'; Align='Left' },
            @{ Header='DEBIT'; Width='0.80in'; Detail=(N2Expr 'Debit' 'DataSet1'); Align='Right' },
            @{ Header='CREDIT'; Width='0.80in'; Detail=(N2Expr 'Credit' 'DataSet1'); Align='Right' },
            @{ Header='BALANCE'; Width='0.80in'; Detail='=""'; Align='Right' }
        )
        FooterRows = @(
            @{ Cells=@('="TOTAL"','=""','=""','=""',(SumExpr 'Debit' 'DataSet1'),(SumExpr 'Credit' 'DataSet1'),'""'); Align=@('Left','Left','Left','Left','Right','Right','Right') }
        )
    },
    @{
        Path = 'FO-2022\Reports\Daily_Transaction_Report.rdlc'
        DataSet = 'DataSet1'
        Columns = @(
            @{ Header='DATE'; Width='0.80in'; Detail='=IIF(IsNothing(Fields!TransactionDate.Value), "", Format(Fields!TransactionDate.Value, "dd-MMM-yyyy"))'; Align='Left' },
            @{ Header='DOC NO'; Width='0.85in'; Detail='=Fields!TransactionNo.Value'; Align='Left' },
            @{ Header='REFERENCE'; Width='1.00in'; Detail='=Fields!TransactionType.Value'; Align='Left' },
            @{ Header='DESCRIPTION'; Width='2.40in'; Detail='=Fields!AccountName.Value & IIF(Len(Trim(CStr(Fields!PartyName.Value))) > 0, " / " & Fields!PartyName.Value, "") & IIF(Len(Trim(CStr(Fields!Description.Value))) > 0, " / " & Fields!Description.Value, "")'; Align='Left' },
            @{ Header='DEBIT'; Width='0.80in'; Detail=(N2Expr 'Debit' 'DataSet1'); Align='Right' },
            @{ Header='CREDIT'; Width='0.80in'; Detail=(N2Expr 'Credit' 'DataSet1'); Align='Right' },
            @{ Header='BALANCE'; Width='0.80in'; Detail='=""'; Align='Right' }
        )
        FooterRows = @(
            @{ Cells=@('="TOTAL"','=""','=""','=""',(SumExpr 'Debit' 'DataSet1'),(SumExpr 'Credit' 'DataSet1'),'""'); Align=@('Left','Left','Left','Left','Right','Right','Right') }
        )
    },
    @{
        Path = 'FO-2022\Reports\DayBook.rdlc'
        DataSet = 'DataSet1'
        Columns = @(
            @{ Header='DATE'; Width='0.80in'; Detail='=IIF(IsNothing(Fields!VoucherDate.Value), "", Format(Fields!VoucherDate.Value, "dd-MMM-yyyy"))'; Align='Left' },
            @{ Header='DOC NO'; Width='0.85in'; Detail='=Fields!VoucherNo.Value'; Align='Left' },
            @{ Header='REFERENCE'; Width='1.00in'; Detail='=Fields!VoucherType.Value'; Align='Left' },
            @{ Header='DESCRIPTION'; Width='2.40in'; Detail='=Fields!AccountNo.Value & " / " & Fields!Narration.Value'; Align='Left' },
            @{ Header='DEBIT'; Width='0.80in'; Detail=(N2Expr 'Debit' 'DataSet1'); Align='Right' },
            @{ Header='CREDIT'; Width='0.80in'; Detail=(N2Expr 'Credit' 'DataSet1'); Align='Right' },
            @{ Header='BALANCE'; Width='0.80in'; Detail='=""'; Align='Right' }
        )
        FooterRows = @(
            @{ Cells=@('="TOTAL"','=""','=""','=""',(SumExpr 'Debit' 'DataSet1'),(SumExpr 'Credit' 'DataSet1'),'""'); Align=@('Left','Left','Left','Left','Right','Right','Right') }
        )
    },
    @{
        Path = 'FO-2022\Reports\Party_Detailed_Ledger_Combine.rdlc'
        DataSet = 'DataSet1'
        Columns = @(
            @{ Header='DATE'; Width='0.80in'; Detail='=Fields!DisplayDate.Value'; Align='Left' },
            @{ Header='DOC NO'; Width='0.85in'; Detail='=Fields!DisplayVoucherNo.Value'; Align='Left' },
            @{ Header='REFERENCE'; Width='1.00in'; Detail='=Fields!DisplayReference.Value'; Align='Left' },
            @{ Header='DESCRIPTION'; Width='2.40in'; Detail='=Fields!DisplayDescription.Value'; Align='Left' },
            @{ Header='DEBIT'; Width='0.80in'; Detail='=IIF(Len(Trim(CStr(Fields!DebitText.Value))) > 0, Fields!DebitText.Value, IIF(IsNothing(Fields!Debit.Value), "", Format(Fields!Debit.Value, "N2")))'; Align='Right' },
            @{ Header='CREDIT'; Width='0.80in'; Detail='=IIF(Len(Trim(CStr(Fields!CreditText.Value))) > 0, Fields!CreditText.Value, IIF(IsNothing(Fields!Credit.Value), "", Format(Fields!Credit.Value, "N2")))'; Align='Right' },
            @{ Header='BALANCE'; Width='0.80in'; Detail='=IIF(Len(Trim(CStr(Fields!BalanceText.Value))) > 0, Fields!BalanceText.Value, IIF(IsNothing(Fields!Balance.Value), "", Format(Fields!Balance.Value, "N2")))'; Align='Right' }
        )
        FooterRows = @(
            @{ Cells=@('="TOTAL"','=""','=""','=""',(SumExpr 'Debit' 'DataSet1'),(SumExpr 'Credit' 'DataSet1'),'""'); Align=@('Left','Left','Left','Left','Right','Right','Right') },
            @{ Cells=@('="CLOSING BALANCE"','=""','=""','=""','=""','""','=Last(Fields!BalanceText.Value, "DataSet1")'); Align=@('Left','Left','Left','Left','Right','Right','Right') }
        )
    },
    @{
        Path = 'FO-2022\Reports\PartyLedgerDetailed.rdlc'
        DataSet = 'LedgerDataSet'
        Columns = @(
            @{ Header='DATE'; Width='0.80in'; Detail='=Fields!DisplayDate.Value'; Align='Left' },
            @{ Header='DOC NO'; Width='0.85in'; Detail='=Fields!VoucherNo.Value'; Align='Left' },
            @{ Header='REFERENCE'; Width='1.00in'; Detail='=Fields!Reference.Value'; Align='Left' },
            @{ Header='DESCRIPTION'; Width='2.40in'; Detail='=Fields!Description.Value'; Align='Left' },
            @{ Header='DEBIT'; Width='0.80in'; Detail=(N2Expr 'DebitAmount' 'LedgerDataSet'); Align='Right' },
            @{ Header='CREDIT'; Width='0.80in'; Detail=(N2Expr 'CreditAmount' 'LedgerDataSet'); Align='Right' },
            @{ Header='BALANCE'; Width='0.80in'; Detail='=IIF(Len(Trim(CStr(Fields!BalanceText.Value))) > 0, Fields!BalanceText.Value, IIF(IsNothing(Fields!BalanceAmount.Value), "", Format(Fields!BalanceAmount.Value, "N2")))'; Align='Right' }
        )
        FooterRows = @(
            @{ Cells=@('="TOTAL"','=""','=""','=""',(SumExpr 'DebitAmount' 'LedgerDataSet'),(SumExpr 'CreditAmount' 'LedgerDataSet'),'""'); Align=@('Left','Left','Left','Left','Right','Right','Right') },
            @{ Cells=@('="CLOSING BALANCE"','=""','=""','=""','=""','""','=Last(Fields!BalanceText.Value, "LedgerDataSet")'); Align=@('Left','Left','Left','Left','Right','Right','Right') }
        )
    },
    @{
        Path = 'FO-2022\Reports\Accounts_Ledger_Category.rdlc'
        DataSet = 'DataSet1'
        Columns = @(
            @{ Header='ACCOUNT NO'; Width='0.75in'; Detail='=Fields!AccountNo.Value'; Align='Left' },
            @{ Header='ACCOUNT NAME'; Width='1.30in'; Detail='=Fields!AccountName.Value'; Align='Left' },
            @{ Header='PARTY NAME'; Width='1.10in'; Detail='=Fields!PartyName.Value'; Align='Left' },
            @{ Header='CATEGORY'; Width='1.10in'; Detail='=Fields!CategoryName.Value'; Align='Left' },
            @{ Header='OPENING BALANCE'; Width='1.00in'; Detail='=Fields!OpeningBalance.Value & " " & Fields!OpeningBalanceType.Value'; Align='Right' },
            @{ Header='DEBIT'; Width='0.70in'; Detail=(N2Expr 'Debit' 'DataSet1'); Align='Right' },
            @{ Header='CREDIT'; Width='0.70in'; Detail=(N2Expr 'Credit' 'DataSet1'); Align='Right' },
            @{ Header='CLOSING BALANCE'; Width='0.80in'; Detail='=Fields!ClosingBalance.Value & " " & Fields!ClosingBalanceType.Value'; Align='Right' }
        )
        FooterRows = @(
            @{ Cells=@('="TOTAL"','=""','=""','=""','"',(SumExpr 'Debit' 'DataSet1'),(SumExpr 'Credit' 'DataSet1'),'""'); Align=@('Left','Left','Left','Left','Right','Right','Right','Right') }
        )
    },
    @{
        Path = 'FO-2022\Reports\Account_Balances.rdlc'
        DataSet = 'DataSet1'
        Columns = @(
            @{ Header='ACCOUNT NO'; Width='0.80in'; Detail='=Fields!AccountNo.Value'; Align='Left' },
            @{ Header='ACCOUNT NAME'; Width='1.30in'; Detail='=Fields!AccountName.Value'; Align='Left' },
            @{ Header='TYPE'; Width='0.85in'; Detail='=Fields!AccountType.Value & IIF(Len(Trim(CStr(Fields!PartyType.Value))) > 0, " / " & Fields!PartyType.Value, "")'; Align='Left' },
            @{ Header='OPENING BALANCE'; Width='1.10in'; Detail=(N2Expr 'OpeningBalance' 'DataSet1'); Align='Right' },
            @{ Header='DEBIT'; Width='0.90in'; Detail=(N2Expr 'TotalDebit' 'DataSet1'); Align='Right' },
            @{ Header='CREDIT'; Width='0.90in'; Detail=(N2Expr 'TotalCredit' 'DataSet1'); Align='Right' },
            @{ Header='CLOSING BALANCE'; Width='1.10in'; Detail=(N2Expr 'CurrentBalance' 'DataSet1'); Align='Right' }
        )
        FooterRows = @(
            @{ Cells=@('="TOTAL"','=""','=""',(SumExpr 'OpeningBalance' 'DataSet1'),(SumExpr 'TotalDebit' 'DataSet1'),(SumExpr 'TotalCredit' 'DataSet1'),(SumExpr 'CurrentBalance' 'DataSet1')); Align=@('Left','Left','Left','Right','Right','Right','Right') }
        )
    }
)

foreach ($target in $targets) {
    $fullPath = Join-Path $PSScriptRoot '..' | Join-Path -ChildPath $target.Path
    $resolved = [System.IO.Path]::GetFullPath($fullPath)
    $tablix = Build-TablixXml -DataSetName $target.DataSet -Columns $target.Columns -FooterRows $target.FooterRows
    Replace-BodyWithTablix -Path $resolved -TablixXml $tablix
}

$files = Get-ChildItem (Join-Path $PSScriptRoot '..\FO-2022') -File -Filter *.rdlc
$files += Get-ChildItem (Join-Path $PSScriptRoot '..\FO-2022\Reports') -File -Filter *.rdlc
$failed = @()
foreach ($file in $files) {
    try {
        $null = [xml](Get-Content -LiteralPath $file.FullName -Raw)
    } catch {
        $failed += "$($file.FullName) :: $($_.Exception.Message)"
    }
}

if ($failed.Count -gt 0) {
    throw ($failed -join "`n")
}

Write-Output "Updated $($targets.Count) ledger-oriented RDLC layouts."
