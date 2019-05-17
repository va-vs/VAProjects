$$.module.address.source.hotel = "@Afghanistan|Afghanistan|1|@Argentina|Argentina|2|@||3|@Belarus|Belarus|4|";
$$.module.address.source.hotel_hotData = {
    "A":"@1|Afghanistan@2|Argentina@3|Armenia@4|Around the World in 500 Festivals@5|Australia@6|Austria@7|Azerbaijan",
    'B':"@8|Bangladesh@9|Belaru@10|Belgium@11|Bolivia@12|Bosnia@13|Botswana@14|Brazil@15|Britain@16|Bulgaria",
	'C':"@17|Cambodia@18|Canada@19|Chile@20|China@21|Chinese Philosophy@22|Colombia @23|Costa Rica@24|Croatia@25|Cuba@26|Czech Republic",
	'DE':"@27|Denmark@28|Dominican Republic@29|Ecuador@30|Egypt@31|Estonia@32|Ethiopia",
	'FG':"@33|Finland@34|France@35|Georgia@36|Germany@37|Ghana@38|Greece@39|Greek Philosophy",
	'HI':"@40|Hungary@41|India@42|Indonesia@43|Iran@44|Ireland@45|Israel@46|Italy",
	'JKL':"@47|Jamaica@48|Japan@49|Kazakhstan@50|Kenya@51|Korea@52|Libya@53|Lithuania",
	'M':"@54|Malaysia@55|Mauritius@56|Mexico@57|Morocco@58|Mongolia@59|Myanmar ",
	'N':"@60|Namibia@61|Nigeria@62|Nepal@63|Netherlands@64|New Zealand@65|Norway",
	'OPQR':"@66|Oman@67|Pakistan@68|Peru@69|Philippines@70|Poland@71|Portugal@72|Romania@73|Russia",
	'S':"@74|Saudi Arabia@75|Scotland@76|Serbia@77|Slovakia@78|Slovenia@79|Singapore@80|South Africa@81|Spain@82|Sri Lanka@83|Sweden@84|Switzerland@85|Syria",
	'T':"@86|Tanzania@87|The Quest for Speed@88|The Theory of Evolution@89|Thailand@90|Trinidad & Tobago@91|Tunisia@92|Turkey",
	'UV':"@93|UAE@94|Uganda@95|Ukraine@96|USA@97|Venezuela@98|Vietnam"
};


String.prototype.cntStr = function() {
    return this.replace(/@+/g, "@");
};
