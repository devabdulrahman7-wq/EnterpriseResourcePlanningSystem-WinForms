$ErrorActionPreference = 'Stop'

function Get-RdlTextRunStyle {
    param(
        [System.Xml.XmlDocument]$Xml,
        [System.Xml.XmlNamespaceManager]$Ns,
        [System.Xml.XmlNode]$Textbox
    )

    $style = $Textbox.SelectSingleNode('def:Paragraphs/def:Paragraph/def:TextRuns/def:TextRun/def:Style', $Ns)
    if (-not $style) {
        $textRun = $Textbox.SelectSingleNode('def:Paragraphs/def:Paragraph/def:TextRuns/def:TextRun', $Ns)
        if (-not $textRun) {
            return $null
        }

        $style = $Xml.CreateElement('Style', $Ns.LookupNamespace('def'))
        [void]$textRun.AppendChild($style)
    }

    return $style
}

function Set-Or-CreateChildText {
    param(
        [System.Xml.XmlDocument]$Xml,
        [System.Xml.XmlNode]$Parent,
        [string]$ChildName,
        [string]$NamespaceUri,
        [string]$Value
    )

    $child = $Parent.SelectSingleNode("def:$ChildName", $script:Ns)
    if (-not $child) {
        $child = $Xml.CreateElement($ChildName, $NamespaceUri)
        [void]$Parent.AppendChild($child)
    }

    $child.InnerText = $Value
    return $child
}

function Set-TextboxStyle {
    param(
        [System.Xml.XmlDocument]$Xml,
        [System.Xml.XmlNamespaceManager]$Ns,
        [System.Xml.XmlNode]$Textbox,
        [string]$HorizontalAlign,
        [string]$VerticalAlign = 'Top',
        [bool]$Bold = $false,
        [string]$FontSize = '8pt'
    )

    $textRunStyle = Get-RdlTextRunStyle -Xml $Xml -Ns $Ns -Textbox $Textbox
    if ($textRunStyle) {
        Set-Or-CreateChildText -Xml $Xml -Parent $textRunStyle -ChildName 'FontFamily' -NamespaceUri $Ns.LookupNamespace('def') -Value 'Arial' | Out-Null
        Set-Or-CreateChildText -Xml $Xml -Parent $textRunStyle -ChildName 'FontSize' -NamespaceUri $Ns.LookupNamespace('def') -Value $FontSize | Out-Null
        if ($Bold) {
            Set-Or-CreateChildText -Xml $Xml -Parent $textRunStyle -ChildName 'FontWeight' -NamespaceUri $Ns.LookupNamespace('def') -Value 'Bold' | Out-Null
        }
    }

    $paragraphStyle = $Textbox.SelectSingleNode('def:Paragraphs/def:Paragraph/def:Style', $Ns)
    if (-not $paragraphStyle) {
        $paragraph = $Textbox.SelectSingleNode('def:Paragraphs/def:Paragraph', $Ns)
        if ($paragraph) {
            $paragraphStyle = $Xml.CreateElement('Style', $Ns.LookupNamespace('def'))
            [void]$paragraph.AppendChild($paragraphStyle)
        }
    }

    if ($paragraphStyle) {
        Set-Or-CreateChildText -Xml $Xml -Parent $paragraphStyle -ChildName 'TextAlign' -NamespaceUri $Ns.LookupNamespace('def') -Value $HorizontalAlign | Out-Null
    }

    $style = $Textbox.SelectSingleNode('def:Style', $Ns)
    if (-not $style) {
        $style = $Xml.CreateElement('Style', $Ns.LookupNamespace('def'))
        [void]$Textbox.AppendChild($style)
    }

    $border = $style.SelectSingleNode('def:Border', $Ns)
    if (-not $border) {
        $border = $Xml.CreateElement('Border', $Ns.LookupNamespace('def'))
        [void]$style.AppendChild($border)
    }

    Set-Or-CreateChildText -Xml $Xml -Parent $border -ChildName 'Color' -NamespaceUri $Ns.LookupNamespace('def') -Value 'Black' | Out-Null
    Set-Or-CreateChildText -Xml $Xml -Parent $border -ChildName 'Style' -NamespaceUri $Ns.LookupNamespace('def') -Value 'Solid' | Out-Null
    Set-Or-CreateChildText -Xml $Xml -Parent $style -ChildName 'VerticalAlign' -NamespaceUri $Ns.LookupNamespace('def') -Value $VerticalAlign | Out-Null
    Set-Or-CreateChildText -Xml $Xml -Parent $style -ChildName 'PaddingLeft' -NamespaceUri $Ns.LookupNamespace('def') -Value '2pt' | Out-Null
    Set-Or-CreateChildText -Xml $Xml -Parent $style -ChildName 'PaddingRight' -NamespaceUri $Ns.LookupNamespace('def') -Value '2pt' | Out-Null
    Set-Or-CreateChildText -Xml $Xml -Parent $style -ChildName 'PaddingTop' -NamespaceUri $Ns.LookupNamespace('def') -Value '2pt' | Out-Null
    Set-Or-CreateChildText -Xml $Xml -Parent $style -ChildName 'PaddingBottom' -NamespaceUri $Ns.LookupNamespace('def') -Value '2pt' | Out-Null
}

function New-TextboxXml {
    param(
        [string]$Name,
        [string]$ValueExpression,
        [string]$Top,
        [string]$Left,
        [string]$Height,
        [string]$Width,
        [string]$TextAlign = 'Left',
        [string]$VerticalAlign = 'Top',
        [bool]$Bold = $true,
        [string]$FontSize = '8pt',
        [bool]$Border = $true,
        [string]$BackgroundColor = ''
    )

@"
<Textbox xmlns="http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition" Name="$Name">
  <CanGrow>true</CanGrow>
  <KeepTogether>true</KeepTogether>
  <Paragraphs>
    <Paragraph>
      <TextRuns>
        <TextRun>
          <Value>$ValueExpression</Value>
          <Style>
            <FontFamily>Arial</FontFamily>
            <FontSize>$FontSize</FontSize>
$(if ($Bold) { '            <FontWeight>Bold</FontWeight>' })
          </Style>
        </TextRun>
      </TextRuns>
      <Style>
        <TextAlign>$TextAlign</TextAlign>
      </Style>
    </Paragraph>
  </Paragraphs>
  <Top>$Top</Top>
  <Left>$Left</Left>
  <Height>$Height</Height>
  <Width>$Width</Width>
  <Style>
    <Border>
      <Color>Black</Color>
      <Style>$(if ($Border) { 'Solid' } else { 'None' })</Style>
    </Border>
$(if ($BackgroundColor) { "    <BackgroundColor>$BackgroundColor</BackgroundColor>" })
    <VerticalAlign>$VerticalAlign</VerticalAlign>
    <PaddingLeft>2pt</PaddingLeft>
    <PaddingRight>2pt</PaddingRight>
    <PaddingTop>2pt</PaddingTop>
    <PaddingBottom>2pt</PaddingBottom>
  </Style>
</Textbox>
"@
}

function Replace-PageSection {
    param(
        [System.Xml.XmlDocument]$Xml,
        [System.Xml.XmlNamespaceManager]$Ns,
        [string]$SectionName,
        [string]$InnerXml
    )

    $page = $Xml.SelectSingleNode('/def:Report/def:Page', $Ns)
    $old = $page.SelectSingleNode("def:$SectionName", $Ns)
    if ($old) {
        [void]$page.RemoveChild($old)
    }

    $fragment = $Xml.CreateDocumentFragment()
    $fragment.InnerXml = "<$SectionName xmlns=`"http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition`">$InnerXml</$SectionName>"

    if ($page.FirstChild) {
        [void]$page.InsertBefore($fragment.FirstChild, $page.FirstChild)
    } else {
        [void]$page.AppendChild($fragment.FirstChild)
    }
}

function Normalize-LegacyReport {
    param(
        [string]$Path,
        [string]$Title,
        [hashtable]$BodyAlignment,
        [string[]]$HeaderExpressions
    )

    $content = Get-Content -LiteralPath $Path -Raw
    $content = $content -replace 'LightGrey', 'Black'

    $xml = New-Object System.Xml.XmlDocument
    $xml.PreserveWhitespace = $true
    $xml.LoadXml($content)

    $ns = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
    $ns.AddNamespace('def', 'http://schemas.microsoft.com/sqlserver/reporting/2008/01/reportdefinition')
    $script:Ns = $ns

    $bodyTextboxes = $xml.SelectNodes('/def:Report/def:Body//def:Textbox', $ns)
    foreach ($textbox in $bodyTextboxes) {
        $name = $textbox.Attributes['Name'].Value
        $isHeader = $name -like 'Textbox*' -and ($name -match '^[A-Za-z]+\d+$' -or $name -in @('Textbox3','Textbox5','Textbox7','Textbox9','Textbox14','Textbox16','Textbox21','Textbox23','Textbox25','Textbox27','Textbox29','Textbox31','Textbox33','Textbox35'))
        $align = if ($BodyAlignment.ContainsKey($name)) { $BodyAlignment[$name] } else { 'Left' }
        Set-TextboxStyle -Xml $xml -Ns $ns -Textbox $textbox -HorizontalAlign $align -VerticalAlign $(if ($isHeader) { 'Middle' } else { 'Top' }) -Bold:$isHeader
    }

    $reportWidth = [double](([regex]::Match($xml.SelectSingleNode('/def:Report/def:Width', $ns).InnerText, '([0-9.]+)').Groups[1].Value))
    $left1Width = [math]::Round($reportWidth * 0.54, 4)
    $right1Left = [math]::Round($left1Width + 0.1, 4)
    $right1Width = [math]::Round($reportWidth - $right1Left, 4)
    $left2Width = [math]::Round(($reportWidth - 0.05) / 2, 4)
    $right2Left = [math]::Round($left2Width + 0.05, 4)
    $right2Width = [math]::Round($reportWidth - $right2Left, 4)
    $footerPageWidth = [math]::Min(1.3, [math]::Round($reportWidth * 0.22, 4))
    $footerPageLeft = [math]::Round($reportWidth - $footerPageWidth, 4)
    $footerTitleWidth = [math]::Round($footerPageLeft - 0.1, 4)

    $headerXml = @"
<Height>0.95in</Height>
<PrintOnFirstPage>true</PrintOnFirstPage>
<PrintOnLastPage>true</PrintOnLastPage>
<ReportItems>
$(New-TextboxXml -Name 'TxtTitle' -ValueExpression $Title -Top '0in' -Left '0in' -Height '0.24in' -Width ("{0}in" -f $reportWidth) -TextAlign 'Center' -VerticalAlign 'Middle' -Bold $true -FontSize '12pt')
$(New-TextboxXml -Name 'TxtLeft1' -ValueExpression $HeaderExpressions[0] -Top '0.34in' -Left '0in' -Height '0.2in' -Width ("{0}in" -f $left1Width))
$(New-TextboxXml -Name 'TxtRight1' -ValueExpression $HeaderExpressions[1] -Top '0.34in' -Left ("{0}in" -f $right1Left) -Height '0.2in' -Width ("{0}in" -f $right1Width))
$(New-TextboxXml -Name 'TxtLeft2' -ValueExpression $HeaderExpressions[2] -Top '0.56in' -Left '0in' -Height '0.2in' -Width ("{0}in" -f $left2Width))
$(New-TextboxXml -Name 'TxtRight2' -ValueExpression $HeaderExpressions[3] -Top '0.56in' -Left ("{0}in" -f $right2Left) -Height '0.2in' -Width ("{0}in" -f $right2Width))
</ReportItems>
<Style>
  <Border>
    <Style>None</Style>
  </Border>
</Style>
"@

    $footerXml = @"
<Height>0.28958in</Height>
<PrintOnFirstPage>true</PrintOnFirstPage>
<PrintOnLastPage>true</PrintOnLastPage>
<ReportItems>
$(New-TextboxXml -Name 'FooterTitle' -ValueExpression $Title -Top '0in' -Left '0in' -Height '0.22in' -Width ("{0}in" -f $footerTitleWidth) -Border $false)
$(New-TextboxXml -Name 'FooterPageText' -ValueExpression '="Page " &amp; Globals!PageNumber &amp; " of " &amp; Globals!TotalPages' -Top '0in' -Left ("{0}in" -f $footerPageLeft) -Height '0.22in' -Width ("{0}in" -f $footerPageWidth) -TextAlign 'Right' -Border $false)
</ReportItems>
<Style>
  <Border>
    <Style>None</Style>
  </Border>
</Style>
"@

    Replace-PageSection -Xml $xml -Ns $ns -SectionName 'PageHeader' -InnerXml $headerXml
    Replace-PageSection -Xml $xml -Ns $ns -SectionName 'PageFooter' -InnerXml $footerXml

    foreach ($margin in 'LeftMargin','RightMargin','TopMargin','BottomMargin') {
        $node = $xml.SelectSingleNode("/def:Report/def:Page/def:$margin", $ns)
        if (-not $node) {
            $node = $xml.CreateElement($margin, $ns.LookupNamespace('def'))
            [void]$xml.SelectSingleNode('/def:Report/def:Page', $ns).AppendChild($node)
        }
        $node.InnerText = '0.25in'
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

$projectRoot = Split-Path -Parent $PSScriptRoot
$projectFile = Join-Path $projectRoot 'FO-2022\FO-2022.csproj'
[xml]$projXml = Get-Content -LiteralPath $projectFile -Raw

$embeddedResources = Select-String -InputObject $projXml.OuterXml -Pattern '<EmbeddedResource Include="([^"]+\.rdlc)"' -AllMatches |
    ForEach-Object { $_.Matches } |
    ForEach-Object {
        $relative = [System.Uri]::UnescapeDataString($_.Groups[1].Value)
        Join-Path (Join-Path $projectRoot 'FO-2022') $relative
    }

foreach ($path in $embeddedResources) {
    if (-not (Test-Path -LiteralPath $path)) {
        continue
    }

    $content = Get-Content -LiteralPath $path -Raw
    $updated = $content -replace 'A c c o u n t N a m e:', 'Account Name:'

    if ($updated -ne $content) {
        [System.IO.File]::WriteAllText($path, $updated, [System.Text.Encoding]::UTF8)
    }
}

Normalize-LegacyReport -Path (Join-Path $projectRoot 'FO-2022\Chart_of_Accounts.rdlc') `
    -Title 'CHART OF ACCOUNTS' `
    -BodyAlignment @{
        Textbox5 = 'Center'; Textbox7 = 'Center'; Textbox3 = 'Center'; Textbox9 = 'Center'; Textbox14 = 'Center'; Textbox16 = 'Center';
        AccountNo = 'Left'; AccountName = 'Left'; AccountType = 'Left'; Narration = 'Left'; OpeningDebit = 'Right'; OpeningCredit = 'Right'
    } `
    -HeaderExpressions @(
        '="Account Name: " &amp; IIF(CountDistinct(Fields!AccountName.Value, "DataSet1") = 1, First(Fields!AccountName.Value, "DataSet1"), "All")',
        '="Printed On: " &amp; Format(Globals!ExecutionTime, "dd-MMM-yyyy hh:mm tt")',
        '="Account Type: " &amp; IIF(CountDistinct(Fields!AccountType.Value, "DataSet1") = 1, First(Fields!AccountType.Value, "DataSet1"), "Multiple")',
        '="User: " &amp; User!UserID'
    )

Normalize-LegacyReport -Path (Join-Path $projectRoot 'FO-2022\Salesman_Summary[report].rdlc') `
    -Title 'SALESMAN SUMMARY REPORT' `
    -BodyAlignment @{
        Textbox21 = 'Center'; Textbox23 = 'Center'; Textbox25 = 'Center'; Textbox27 = 'Center'; Textbox29 = 'Center'; Textbox31 = 'Center'; Textbox33 = 'Center'; Textbox35 = 'Center';
        CustomerID = 'Left'; CustomerName = 'Left'; SaleValue = 'Right'; ReturnValue = 'Right'; NetValue = 'Right'; ReturnPercentage = 'Right'; ReceivedAmount = 'Right'; BalanceAmount = 'Right'
    } `
    -HeaderExpressions @(
        '="Customer Name: " &amp; IIF(CountDistinct(Fields!CustomerName.Value, "DataSet1") = 1, First(Fields!CustomerName.Value, "DataSet1"), "Multiple Customers")',
        '="Printed On: " &amp; Format(Globals!ExecutionTime, "dd-MMM-yyyy hh:mm tt")',
        '="Customer ID: " &amp; IIF(CountDistinct(Fields!CustomerID.Value, "DataSet1") = 1, First(Fields!CustomerID.Value, "DataSet1"), "Multiple")',
        '="User: " &amp; User!UserID'
    )

$validationFailures = @()
foreach ($path in $embeddedResources | Sort-Object -Unique) {
    try {
        $null = [xml](Get-Content -LiteralPath $path -Raw)
    } catch {
        $validationFailures += [pscustomobject]@{
            Path = $path
            Error = $_.Exception.Message
        }
    }
}

if ($validationFailures.Count -gt 0) {
    $validationFailures | Format-Table -AutoSize | Out-String | Write-Error
}

Write-Output "Normalized $($embeddedResources.Count) source RDLC references."
