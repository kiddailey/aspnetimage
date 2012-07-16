<%
		Response.Write "Canvas<br/>"
		Dim img
		Dim canvas
		Set img = Server.CreateObject("AspImage.Image")
		Set canvas = Server.CreateObject("AspImage.Image")

		Dim strFileName
		strFileName = Server.MapPath("resized.jpg")

		Dim intResizeWidth
		Dim intResizeHeight
		
		intWidth = 1024
		intHeight = 768
		
		img.LoadImage Server.MapPath("prova.jpg")

		If img.MaxX <> 0 And img.MaxY <> 0 Then
			If ((intWidth/intHeight) <= (img.MaxX/img.MaxY)) Then
				intResizeWidth = intWidth
				intResizeHeight = intWidth * img.MaxY / img.MaxX
			Else
				intResizeWidth = intHeight * img.MaxX / img.MaxY
				intResizeHeight = intHeight
			End If
		
			img.ResizeR intResizeWidth, intResizeHeight
		End If

		img.FileName = strFileName
		img.SaveImage
		Set img = Nothing

		canvas.BackgroundColor = vbWhite
		canvas.MaxX = intWidth
		canvas.MaxY = intHeight
		canvas.AddImage strFileName, Int((intWidth - intResizeWidth) / 2) + 2, Int((intHeight - intResizeHeight) / 2) + 2

		canvas.FileName = Server.MapPath("canvas.jpg")
		canvas.SaveImage

		Set canvas = Nothing
		Response.End
		
		Response.write "LoadImage<br/>"
		Set Image = Server.CreateObject("AspImage.Image")
		oggettoimmagine="\"&trim(request.querystring("img"))
		immagine =server.mappath(oggettoimmagine)
		Image.LoadImage (immagine)
		Width = Image.MaxX
		Height = Image.MaxY
		Set Image = nothing
		Response.End
	
	connstr = "Provider=SQLOLEDB.4;Server=(local);uid=turismo;pwd=$!tur1sm039;database=turismo2008"
	Set conn = Server.CreateObject ("ADODB.Connection")
	conn.Open connstr

		pk_immagine = Request.QueryString("pk_immagine")
		If ((Request.QueryString("width") <> "") Or (Request.QueryString("height") <> "")) Then
			If (Request.QueryString("width") <> "") Then
				intWidth = CInt(Request.QueryString("width"))
			End If

			If (Request.QueryString("height") <> "") Then
				intHeight = CInt(Request.QueryString("height"))
			End If
		End If
			
				Response.Clear
				Response.Expires = -1
				Response.ContentType = "image/jpeg"
				Response.AddHeader "cache-control", "must-revalidate"
				Response.AddHeader "Content-disposition", "inline; filename=" & strFileName & "." & strEst	
						
						Set img = Server.CreateObject("AspImage.Image")
						Set rsTemp = conn.Execute("SELECT immagine FROM tab_immagini WHERE pk_immagine = " & pk_immagine)
						img.LoadBlob rsTemp("immagine").Value, 1
						rsTemp.Close
						Set rsTemp = Nothing
					
						If intHeight < img.MaxY And intWidth < img.MaxX Then
							If (intHeight = 0) Then
								intResizeWidth = intWidth
								intResizeHeight = intWidth * img.MaxY / img.MaxX
							ElseIf (intWidth = 0) Then
								intResizeHeight = intHeight
								intResizeWidth = intHeight * img.MaxX / img.MaxY
							ElseIf ((intWidth/intHeight) <= (img.MaxX/img.MaxY)) Then
								intResizeWidth = intWidth
								intResizeHeight = intWidth * img.MaxY / img.MaxX
							Else
								intResizeWidth = intHeight * img.MaxX / img.MaxY
								intResizeHeight = intHeight
							End If
			
							img.ResizeR intResizeWidth, intResizeHeight
							img.ImageFormat = 1 'jpeg
							img.JPEGQuality = 85
							img.ProgressiveJPEGEncoding = True
						End If
						
						binImage = img.Image
						Response.BinaryWrite binImage				
	'Chiudo la connessione
	conn.Close
	Set conn = Nothing

	Response.End
	
%>
