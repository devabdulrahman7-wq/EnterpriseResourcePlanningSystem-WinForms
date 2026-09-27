$ErrorActionPreference = 'Stop'

$reportsDir = 'e:\FO-2022\FO-2022\Reports'
$skipReports = @(
    'PartyProductSalesLedger_Professional.rdlc'
)

function XmlEscape([string]$value) {
    if ($null -eq $value) { return '' }
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

function New-ParagraphTextbox($name, $value, $align = 'Left', $bold = $false, $borderColor = 'Black', $fontSize = '8pt', $verticalAlign = 'Top', $top = $null, $left = $null, $height = $null, $width = $null) {
    $fontWeight = if ($bold) { '<FontWeight>Bold</FontWeight>' } else { '' }
    $position = ''
    if ($top -ne $null) { $position += "<Top>$top</Top>" }
    if ($left -ne $null) { $position += "<Left>$left</Left>" }
    if ($height -ne $null) { $position += "<Height>$height</Height>" }
    if ($width -ne $null) { $position += "<Width>$width</Width>" }

    return @"
<Textbox Name="$(XmlEscape $name)">
  <CanGrow>true</CanGrow>
  <KeepTogether>true</KeepTogether>
  <Paragraphs>
    <Paragraph>
      <TextRuns>
        <TextRun>
          <Value>$(XmlEscape $value)</Value>
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

function New-CellTextbox($name, $value, $align = 'Left', $bold = $false, $borderColor = 'Black', $verticalAlign = 'Top') {
    $fontWeight = if ($bold) { '<FontWeight>Bold</FontWeight>' } else { '' }
    return @"
<Textbox Name="$(XmlEscape $name)">
  <CanGrow>true</CanGrow>
  <KeepTogether>true</KeepTogether>
  <Paragraphs>
    <Paragraph>
      <TextRuns>
        <TextRun>
          <Value>$(XmlEscape $value)</Value>
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

function Get-Nodes($xml, $xpath) {
    return @($xml.SelectNodes($xpath))
}

function Get-Node($xml, $xpath) {
    return $xml.SelectSingleNode($xpath)
}

function Convert-ToTitle([string]$name) {
    $base = [System.IO.Path]::GetFileNameWithoutExtension($name)
    $base = $base -replace '\[report\]', ''
    $base = $base -replace '[_\(\)]', ' '
    $base = $base -replace '\s+', ' '
    return $base.Trim().ToUpperInvariant()
}

function Get-FriendlyFieldName([string]$name) {
    if ([string]::IsNullOrWhiteSpace($name)) { return '' }
    $text = $name -replace '_', ' '
    $text = $text -replace '(?<=[a-z0-9])(?=[A-Z])', ' '
    $text = $text -replace '(?<=[A-Z])(?=[A-Z][a-z])', ' '
    $text = $text -replace '(?<=[A-Za-z])(?=\d)', ' '
    $text = $text -replace '(?<=\d)(?=[A-Za-z])', ' '
    $text = $text -replace '\s+', ' '
    return $text.Trim()
}

function Get-ColumnHeader($field, $role) {
    $name = $field.Name

    if ($name -match 'ProductDetail|Description|Narration|Shipment|Remarks|Detail') { return 'DESCRIPTION' }
    elseif ($name -match 'Reference|Refrence') { return 'REFERENCE' }
    elseif ($name -match 'VoucherNo|InvoiceNo|SaleId|PurchaseId|ReturnId|PurReturnId|RecoveryID|RecoveryId') { return 'DOC NO' }
    elseif ($name -match '^AccountNo$') { return 'ACCOUNT NO' }
    elseif ($name -match '^ProductID$') { return 'PRODUCT ID' }
    elseif ($name -match '^CustomerName$') { return 'CUSTOMER NAME' }
    elseif ($name -match '^PartyName$') { return 'PARTY NAME' }
    elseif ($name -match '^VendorName$') { return 'VENDOR NAME' }
    elseif ($name -match '^AccountName$') { return 'ACCOUNT NAME' }
    elseif ($name -match '^CompanyName$') { return 'COMPANY NAME' }
    elseif ($name -match '^ProductName$') { return 'PRODUCT NAME' }
    elseif ($name -match '^SalesmanName$') { return 'SALESMAN NAME' }
    elseif ($name -match 'TownName|Town') { return 'TOWN' }
    elseif ($name -match 'Sector') { return 'SECTOR' }
    elseif ($name -match 'Debit') { return 'DEBIT' }
    elseif ($name -match 'Credit') { return 'CREDIT' }
    elseif ($name -match 'Balance') { return 'BALANCE' }
    elseif ($name -match 'NetValue|NetAmount') { return 'NET AMOUNT' }
    elseif ($name -match 'GrossValue|GrossAmount') { return 'GROSS AMOUNT' }
    elseif ($name -match 'TotalValue|Amount|SalePrice|PurchasePrice|RetailPrice|Price') { return 'AMOUNT' }
    elseif ($name -match 'TotalQty|Qty|Quantity|Stock') { return 'QTY' }
    elseif ($name -match 'Date|DisplayDate') { return 'DATE' }

    switch ($role) {
        'date' { return 'DATE' }
        'id' { return 'DOC NO' }
        'name' { return (Get-FriendlyFieldName $name).ToUpperInvariant() }
        'detail' { return 'DESCRIPTION' }
        'qty' { return 'QTY' }
        'amount' { return 'AMOUNT' }
        default { return (Get-FriendlyFieldName $name).ToUpperInvariant() }
    }
}

function Find-Field($fields, [string[]]$preferredNames, [string[]]$regexes = @(
    'PartyProductSalesLedger_Professional.rdlc'
), [string[]]$excludeNames = @(
    'PartyProductSalesLedger_Professional.rdlc'
)) {
    foreach ($preferred in $preferredNames) {
        $match = $fields | Where-Object { $_.Name -eq $preferred -and $_.Name -notin $excludeNames } | Select-Object -First 1
        if ($match) { return $match }
    }

    foreach ($regex in $regexes) {
        $match = $fields | Where-Object { $_.Name -match $regex -and $_.Name -notin $excludeNames } | Select-Object -First 1
        if ($match) { return $match }
    }

    return $null
}

function Get-FieldRole($field) {
    $name = $field.Name
    $type = $field.TypeName

    if ($name -match 'DisplayDate|SaleDate|VoucherDate|ReturnDate|InvoiceDate|RecoveryDate|Date$' -or $type -match 'DateTime') {
        return 'date'
    }
    if ($name -match 'ProductDetail|Description|Narration|Shipment|Remarks|Reference|Refrence|VoucherType|PaymentMode|Type|Detail') {
        return 'detail'
    }
    if ($name -match 'CustomerName|PartyName|VendorName|AccountName|CompanyName|ProductName|SalesmanName|TownName|Sector|BankName|EmployeeName|Name$') {
        return 'name'
    }
    if ($name -match 'VoucherNo|InvoiceNo|InvoiceId|SaleId|PurchaseId|ReturnId|PurReturnId|RecoveryID|RecoveryId|AccountNo|ProductID|CustomerID|VendorID|ID$|No$') {
        return 'id'
    }
    if ($name -match 'Qty|Quantity|Stock|Days|Ratio|Percent|Pcs|Pack' -or $type -match 'Int|Decimal|Double|Single') {
        if ($name -match 'Amount|Value|Price|Debit|Credit|Balance|Total|Net|Gross|Discount|Tax|Receivable|Received|Sales|Profit|Expense|Limit|Opening|Closing') {
            return 'amount'
        }
        return 'qty'
    }
    if ($name -match 'Amount|Value|Price|Debit|Credit|Balance|Total|Net|Gross|Discount|Tax|Receivable|Received|Sales|Profit|Expense|Limit|Opening|Closing') {
        return 'amount'
    }
    return 'text'
}

function Get-FieldExpression($field, $role) {
    $name = $field.Name
    $type = $field.TypeName

    if ($name -match 'Display.*|.*Text$') {
        return '=Fields!' + $name + '.Value'
    }

    if ($role -eq 'date') {
        return '=IIF(IsNothing(Fields!' + $name + '.Value), "", Format(Fields!' + $name + '.Value, "dd-MMM-yyyy"))'
    }

    if ($type -match 'Decimal|Double|Single') {
        if ($role -eq 'qty') {
            return '=IIF(IsNothing(Fields!' + $name + '.Value), "", Format(Fields!' + $name + '.Value, "#,##0.##"))'
        }
        return '=IIF(IsNothing(Fields!' + $name + '.Value), "", Format(Fields!' + $name + '.Value, "N2"))'
    }

    if ($type -match 'Int16|Int32|Int64') {
        return '=IIF(IsNothing(Fields!' + $name + '.Value), "", Format(Fields!' + $name + '.Value, "#,##0"))'
    }

    return '=Fields!' + $name + '.Value'
}

function Normalize-Columns($columns) {
    $sum = ($columns | Measure-Object -Property BaseWidth -Sum).Sum
    if ($sum -le 0) { return $columns }

    $factor = 7.45 / [double]$sum
    for ($i = 0; $i -lt $columns.Count; $i++) {
        $width = [math]::Round($columns[$i].BaseWidth * $factor, 2)
        $columns[$i].Width = ('{0:0.00}in' -f $width)
    }
    return $columns
}

function Select-Columns($fields) {
    $selected = New-Object System.Collections.Generic.List[object]
    $used = New-Object System.Collections.Generic.HashSet[string]

    $candidates = @(
        (Find-Field $fields @('SaleDate','VoucherDate','InvoiceDate','ReturnDate','RecoveryDate','Date','DisplayDate') @() @())
        (Find-Field $fields @('VoucherNo','InvoiceNo','SaleId','PurchaseId','ReturnId','PurReturnId','AccountNo','ProductID') @() $used)
        (Find-Field $fields @('CustomerName','PartyName','VendorName','AccountName','CompanyName','ProductName','SalesmanName','TownName','Sector') @() $used)
        (Find-Field $fields @('ProductDetail','Description','Narration','Shipment','Reference','Refrence','VoucherType','PaymentMode') @() $used)
        (Find-Field $fields @('TotalQty','Qty','Quantity','CurrentBalance','Balance','NetValue','NetAmount','Amount','TotalValue','SalePrice','PurchasePrice','RetailPrice','Debit','Credit') @() $used)
    )

    foreach ($field in $candidates) {
        if ($field -and -not $used.Contains($field.Name)) {
            $role = Get-FieldRole $field
            $selected.Add([pscustomobject]@{
                Field = $field
                Role = $role
            })
            $used.Add($field.Name) | Out-Null
        }
    }

    $remaining = $fields | Where-Object { -not $used.Contains($_.Name) -and $_.Name -notmatch '^RowType$|^LineNo$' }
    foreach ($field in $remaining) {
        if ($selected.Count -ge 7) { break }
        $role = Get-FieldRole $field
        $selected.Add([pscustomobject]@{
            Field = $field
            Role = $role
        })
        $used.Add($field.Name) | Out-Null
    }

    if ($selected.Count -lt 4) {
        foreach ($field in $fields) {
            if ($selected.Count -ge 5) { break }
            if (-not $used.Contains($field.Name)) {
                $role = Get-FieldRole $field
                $selected.Add([pscustomobject]@{
                    Field = $field
                    Role = $role
                })
                $used.Add($field.Name) | Out-Null
            }
        }
    }

    $columns = foreach ($item in $selected) {
        $field = $item.Field
        $role = $item.Role
        $baseWidth = switch ($role) {
            'date' { 0.90 }
            'id' { 0.95 }
            'name' { 1.55 }
            'detail' { 3.10 }
            'qty' { 0.90 }
            'amount' { 0.95 }
            default { 1.10 }
        }

        [pscustomobject]@{
            Header = (Get-ColumnHeader $field $role)
            Expression = Get-FieldExpression $field $role
            Align = if ($role -in @('qty','amount')) { 'Right' } else { 'Left' }
            BaseWidth = $baseWidth
            Width = ''
        }
    }

    return Normalize-Columns @($columns)
}

function Get-HeaderValue($parameters, $fields, [string]$type) {
    $paramNames = @($parameters | ForEach-Object { $_ })
    $dateField = Find-Field $fields @('SaleDate','VoucherDate','InvoiceDate','ReturnDate','RecoveryDate','Date','DisplayDate') @() @()
    $nameField = Find-Field $fields @('PartyName','CustomerName','VendorName','AccountName','CompanyName','ProductName','SalesmanName','TownName','Sector','EmployeeName','BankName') @() @()

    switch ($type) {
        'left1' {
            if ($paramNames -contains 'AccountName') { return '= "Account Name: " & Parameters!AccountName.Value' }
            if ($paramNames -contains 'PartyName') { return '= "Party Name: " & Parameters!PartyName.Value' }
            if ($paramNames -contains 'CustomerName') { return '= "Customer Name: " & Parameters!CustomerName.Value' }
            if ($paramNames -contains 'CompanyName') { return '= "Company Name: " & Parameters!CompanyName.Value' }
            if ($paramNames -contains 'PriceType') { return '= "Price Type: " & Parameters!PriceType.Value' }
            if ($nameField) { return '= "' + (Get-FriendlyFieldName $nameField.Name) + ': " & IIF(CountDistinct(Fields!' + $nameField.Name + '.Value, "' + $script:currentDataSetName + '") = 1, First(Fields!' + $nameField.Name + '.Value, "' + $script:currentDataSetName + '"), "All")' }
            return '= "Filter: All"'
        }
        'left2' {
            if ($paramNames -contains 'FromDate') { return '= "From Date: " & Parameters!FromDate.Value' }
            if ($dateField) { return '= "From Date: " & IIF(CountRows("' + $script:currentDataSetName + '") = 0, "", Format(Min(Fields!' + $dateField.Name + '.Value, "' + $script:currentDataSetName + '"), "dd-MMM-yyyy"))' }
            return '= "From Date: "'
        }
        'left3' {
            if ($paramNames -contains 'ToDate') { return '= "To Date: " & Parameters!ToDate.Value' }
            if ($dateField) { return '= "To Date: " & IIF(CountRows("' + $script:currentDataSetName + '") = 0, "", Format(Max(Fields!' + $dateField.Name + '.Value, "' + $script:currentDataSetName + '"), "dd-MMM-yyyy"))' }
            return '= "To Date: "'
        }
        'right1' {
            if ($paramNames -contains 'PrintedOn') { return '= "Printed On: " & Parameters!PrintedOn.Value' }
            return '= "Printed On: " & Format(Globals!ExecutionTime, "dd-MMM-yyyy hh:mm tt")'
        }
        'right2' {
            if ($paramNames -contains 'PrintedBy') { return '= "User: " & Parameters!PrintedBy.Value' }
            return '= "User: " & User!UserID'
        }
    }
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
    } else { '' }

    $columnXml = ($spec.Columns | ForEach-Object { "                <TablixColumn><Width>$($_.Width)</Width></TablixColumn>" }) -join "`r`n"

    $headerCells = for ($i = 0; $i -lt $spec.Columns.Count; $i++) {
        $col = $spec.Columns[$i]
@"
                    <TablixCell>
                      <CellContents>
                        $(New-CellTextbox -name "Hdr$i" -value $col.Header -align "Center" -bold $true -verticalAlign "Middle")
                      </CellContents>
                    </TablixCell>
"@
    }

    $detailCells = for ($i = 0; $i -lt $spec.Columns.Count; $i++) {
        $col = $spec.Columns[$i]
@"
                    <TablixCell>
                      <CellContents>
                        $(New-CellTextbox -name "Dtl$i" -value $col.Expression -align $col.Align -verticalAlign "Top")
                      </CellContents>
                    </TablixCell>
"@
    }

    $tablixMembers = ($spec.Columns | ForEach-Object { '                <TablixMember />' }) -join "`r`n"

    $headerItems = @(
        (New-ParagraphTextbox -name 'TxtTitle' -value $spec.Title -align 'Center' -bold $true -fontSize '12pt' -verticalAlign 'Middle' -top '0in' -left '0in' -width '7.45in' -height '0.24in')
        (New-ParagraphTextbox -name 'TxtLeft1' -value $spec.HeaderLeft1 -bold $true -top '0.34in' -left '0in' -width '4.05in' -height '0.2in')
        (New-ParagraphTextbox -name 'TxtRight1' -value $spec.HeaderRight1 -bold $true -top '0.34in' -left '4.15in' -width '3.3in' -height '0.2in')
        (New-ParagraphTextbox -name 'TxtLeft2' -value $spec.HeaderLeft2 -bold $true -top '0.56in' -left '0in' -width '2in' -height '0.2in')
        (New-ParagraphTextbox -name 'TxtLeft3' -value $spec.HeaderLeft3 -bold $true -top '0.56in' -left '2.05in' -width '2in' -height '0.2in')
        (New-ParagraphTextbox -name 'TxtRight2' -value $spec.HeaderRight2 -bold $true -top '0.56in' -left '4.15in' -width '3.3in' -height '0.2in')
    )
    $headerXml = ($headerItems -join "`r`n")

    $footerPage = '="Page " & Globals!PageNumber & " of " & Globals!TotalPages'

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
        $(New-ParagraphTextbox -name 'FooterTitle' -value $spec.Title -bold $true -top '0in' -left '0in' -width '3in' -height '0.2in' -borderColor 'White')
        $(New-ParagraphTextbox -name 'FooterPage' -value $footerPage -align 'Right' -bold $true -top '0in' -left '5.8in' -width '1.65in' -height '0.2in' -borderColor 'White')
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
      <rd:DataSourceID>$([guid]::NewGuid().ToString())</rd:DataSourceID>
    </DataSource>
  </DataSources>
$parameterXml
  <DataSets>
    <DataSet Name="$($spec.DataSetName)">
      <Fields>
$fieldXml
      </Fields>
      <Query>
        <DataSourceName>$($spec.DataSourceName)</DataSourceName>
        <CommandText>/* Local Query */</CommandText>
      </Query>
    </DataSet>
  </DataSets>
  <Language>en-US</Language>
  <ConsumeContainerWhitespace>true</ConsumeContainerWhitespace>
  <rd:ReportUnitType>Inch</rd:ReportUnitType>
  <rd:ReportID>$([guid]::NewGuid().ToString())</rd:ReportID>
</Report>
"@

    Set-Content -Path $spec.Path -Value $reportXml -Encoding UTF8
}

function Get-ReportSpec($path) {
    [xml]$xml = Get-Content $path -Raw
    $dataSourceNode = Get-Node $xml "//*[local-name()='DataSource'][1]"
    $dataSetNode = Get-Node $xml "//*[local-name()='DataSet'][1]"
    $paramNodes = Get-Nodes $xml "//*[local-name()='ReportParameter']"
    $fieldNodes = Get-Nodes $xml "//*[local-name()='DataSet'][1]/*[local-name()='Fields']/*[local-name()='Field']"

    $fields = foreach ($fieldNode in $fieldNodes) {
        $typeNode = $fieldNode.SelectSingleNode("./*[local-name()='TypeName']")
        $dataFieldNode = $fieldNode.SelectSingleNode("./*[local-name()='DataField']")
        [pscustomobject]@{
            Name = $fieldNode.Name
            DataField = if ($dataFieldNode) { $dataFieldNode.InnerText } else { $fieldNode.Name }
            TypeName = if ($typeNode) { $typeNode.InnerText } else { 'System.String' }
        }
    }

    if (-not $dataSetNode -or $fields.Count -eq 0) {
        return $null
    }

    $script:currentDataSetName = $dataSetNode.Name
    $parameters = @($paramNodes | ForEach-Object { $_.Name })
    $columns = Select-Columns $fields
    $title = if ($parameters -contains 'ReportTitle') { '=Parameters!ReportTitle.Value' } else { Convert-ToTitle ([System.IO.Path]::GetFileName($path)) }

    return @{
        Path = $path
        Title = $title
        DataSourceName = if ($dataSourceNode) { $dataSourceNode.Name } else { 'LocalDataSource' }
        DataSetName = $dataSetNode.Name
        Parameters = $parameters
        HeaderLeft1 = Get-HeaderValue $parameters $fields 'left1'
        HeaderLeft2 = Get-HeaderValue $parameters $fields 'left2'
        HeaderLeft3 = Get-HeaderValue $parameters $fields 'left3'
        HeaderRight1 = Get-HeaderValue $parameters $fields 'right1'
        HeaderRight2 = Get-HeaderValue $parameters $fields 'right2'
        Fields = $fields
        Columns = $columns
    }
}

$targets = Get-ChildItem -Path $reportsDir -Filter *.rdlc |
    Where-Object { $_.Name -notin $skipReports } |
    Sort-Object Name

$updated = @(
    'PartyProductSalesLedger_Professional.rdlc'
)
foreach ($target in $targets) {
    $spec = Get-ReportSpec $target.FullName
    if ($null -eq $spec) {
        Write-Warning "Skipped $($target.Name) because no compatible dataset contract was found."
        continue
    }

    New-ClassicReport $spec
    $updated += $target.Name
    Write-Host "Standardized $($target.Name)"
}

Write-Host ""
Write-Host "Total standardized: $($updated.Count)"




