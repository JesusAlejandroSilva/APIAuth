Para ejecutar el API se debe limpiar y recompilar el proyecto, si es necesario restaurar paquetes Nugets 
y Ajustar desde el AppSettings tu cadena de conexion a la base de datos con la tabla que indicaba la prueba
crear en este caso utilize SQL server ya que por tiempo MYSQL no lo tenia instalado. 

CREATE DataBase UsersLog;
Use DataBase;
CREATE TABLE dbo.login_log (
    id INT IDENTITY(1,1) PRIMARY KEY,
    username NVARCHAR(200) NOT NULL,
    login_time DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    access_token NVARCHAR(MAX) NULL
);
