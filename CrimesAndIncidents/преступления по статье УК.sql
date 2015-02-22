SELECT story, A.shortName,  C.number, C.part FROM Crime Cr 
    LEFT JOIN Clause C ON Cr.idClause = C.idClause
    LEFT JOIN Portaking P ON P.idCrime = Cr.idCrime
    LEFT JOIN Accomplice A ON A.idAccomplice = P.idAccomplice
WHERE C.number = 260