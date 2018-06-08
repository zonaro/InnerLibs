ALTER FUNCTION Fonema
(
	@Palavra VARCHAR(MAX) = ''
)
RETURNS VARCHAR(MAX)
AS
BEGIN

	DECLARE @text VARCHAR(MAX) = ISNULL(@Palavra,'')

    SET  @text = UPPER(LTRIM(RTRIM(@text)))
    IF (@text LIKE '%Z' And @text LIKE 'Z%') 
		BEGIN
		 
				WHILE @text LIKE '%Z'
				BEGIN
					SET @text = LEFT(@text, LEN(@text) - 1)
				END

				WHILE @text LIKE 'Z%'
				BEGIN
					SET @text = RIGHT(@text, LEN(@text) - 1)
				END

			    SET  @text = 'Z' + REPLACE(@text,'Z','S') + 'S'
         END         
             
    ELSE
        BEGIN
			 If (@text LIKE  'Z%') 
			 BEGIN
			    SET @text = 'Z' + replace(@text,'Z', 'S')
             END
        ELSE
             BEGIN
			   SET @text = replace(@text,'Z', 'S')
             END  
        END           
               
          
          set @text = replace(@text,'Ç', 'S')        
          set @text = replace(@text,'Y', 'I')	 
          set @text = replace(@text,'AL', 'AU')
          set @text = replace(@text,'CC', 'T')
          set @text = replace(@text,'ZZ', 'TS')
          set @text = replace(@text,'BR', 'B')
          set @text = replace(@text,'BL', 'B')
          set @text = replace(@text,'PH', 'F')
          set @text = replace(@text,'MG', 'G')
          set @text = replace(@text,'NG', 'G')
          set @text = replace(@text,'RG', 'G')
          set @text = replace(@text,'GE', 'J')
          set @text = replace(@text,'GI', 'J')
          set @text = replace(@text,'RJ', 'J')
          set @text = replace(@text,'MJ', 'J')
          set @text = replace(@text,'NJ', 'J')
          set @text = replace(@text,'GR', 'G')
          set @text = replace(@text,'GL', 'G')
          set @text = replace(@text,'CE', 'S')
          set @text = replace(@text,'CI', 'S')
          set @text = replace(@text,'CH', 'X')
          set @text = replace(@text,'CT', 'T')
          set @text = replace(@text,'CS', 'S')
          set @text = replace(@text,'QU', 'K')
          set @text = replace(@text,'Q', 'K')
          set @text = replace(@text,'CA', 'K')
          set @text = replace(@text,'CO', 'K')
          set @text = replace(@text,'CU', 'K')
          set @text = replace(@text,'CK', 'K')
          set @text = replace(@text,'LH', 'LI')
          set @text = replace(@text,'RM', 'SM')
          set @text = replace(@text,'N', 'M')
          set @text = replace(@text,'GM', 'M')
          set @text = replace(@text,'MD', 'M')
          set @text = replace(@text,'NH', 'N')
          set @text = replace(@text,'PR', 'P')
          set @text = replace(@text,'X', 'S')
          set @text = replace(@text,'TS', 'S')
          set @text = replace(@text,'RS', 'S')
          set @text = replace(@text,'TR', 'T')
          set @text = replace(@text,'TL', 'T')
          set @text = replace(@text,'LT', 'T')
          set @text = replace(@text,'RT', 'T')
          set @text = replace(@text,'ST', 'T')
          set @text = replace(@text,'W', 'V')
          set @text = replace(@text,'L', 'R')
          set @text = replace(@text,'H', '')  
     
		   DECLARE @letras VARCHAR(MAX) =  'ABCDEFGHIJKLMNOPQRSTUVWXYZ'
		   DECLARE @cnt2 INT = 0
		   WHILE @cnt2 < LEN(@letras)
		   BEGIN	 
		   	DECLARE @letra char(1) = SUBSTRING(@letras,@cnt2,1)  
		   	WHILE @text LIKE '%'+ @letra + @letra  +'%'
		   	BEGIN
		   		SET @text = REPLACE(@text,@letra+@letra,@letra)
		       END 
		   	SET @cnt2 = @cnt2+1
		   END


		  DECLARE @sb varchar(MAX) = @text

            If (@text <> '') 
			BEGIN     
                declare @tam int = LEN(@sb) - 1
                If (@tam > -1) 
				BEGIN                
                    IF(SUBSTRING(@sb, @tam, 1) = 'S' or SUBSTRING(@sb, @tam, 1) = 'Z' or SUBSTRING(@sb, @tam, 1) = 'R' or SUBSTRING(@sb, @tam, 1) = 'M' or SUBSTRING(@sb, @tam, 1) = 'N' or SUBSTRING(@sb, @tam, 1) = 'L')
                    BEGIN
						set @sb =  STUFF(@sb, @tam, 1, '')					   
                    END
                END
                
                set  @tam = LEN(@sb) - 2
                If (@tam > -1) 
				BEGIN
					IF (SUBSTRING(@sb, @tam, 1) = 'A' AND SUBSTRING(@sb, @tam+1, 1) = 'O' )
						BEGIN
						   set @sb =  STUFF(@sb, @tam, 2, '')		
					    END  
                END
                 
                declare @frasesaida varchar(MAX) = ''           
                
			    
                SET @frasesaida = @frasesaida + SUBSTRING(@sb, 1, 1)
			 


				DECLARE @cnt INT = 1;

				WHILE @cnt < (LEN(@sb) - 1)
				BEGIN
			
			    IF(SUBSTRING(@frasesaida, LEN(@frasesaida) - 1,1) <> SUBSTRING(@sb, @cnt,1) or SUBSTRING(@sb, @cnt,1) NOT IN ('1','2','3','4','5','6','7','8','9','0') )
					BEGIN
					   SET  @frasesaida = @frasesaida + SUBSTRING(@sb, @cnt, 1)
					END
				   SET @cnt = @cnt + 1;
				END
                RETURN @frasesaida 
			END

	RETURN @text
END

GO

ALTER FUNCTION CompararFonema
(
	@Palavra1 VARCHAR(MAX) = '',
	@Palavra2 VARCHAR(MAX) = ''
)
RETURNS bit
AS
BEGIN
IF(dbo.Fonema(@Palavra1) = dbo.Fonema(@Palavra2))
BEGIN	
RETURN 1
END
RETURN 0
END	