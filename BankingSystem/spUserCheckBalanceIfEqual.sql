USE [Banking]
GO
/****** Object:  StoredProcedure [dbo].[spUserCheckBalanceIfEqual]    Script Date: 6/13/2018 9:35:32 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[spUserCheckBalanceIfEqual]
(      
   @AccountNumber nchar(20)
)     
as    
Begin    
    select Balance    
    from users  where   AccountNumber = @AccountNumber 
End
GO
