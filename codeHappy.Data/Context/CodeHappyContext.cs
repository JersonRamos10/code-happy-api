using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
namespace codeHappy.Data.Context;

public class CodeHappyContext : DbContext
{
    public CodeHappyContext(DbContextOptions<CodeHappyContext> options) : base(options)
    {

    }



}