using Ecom.API.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<rise_ReportDetail> rise_ReportDetails { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<rise_order> rise_orders { get; set; }
    public DbSet<Income> Incomes { get; set; }
    public DbSet<Card> Cards { get; set; }
    public DbSet<CardPhoto> CardsPhotos { get; set; }
    public DbSet<CardSize> CardsSizes { get; set; }

    public DbSet<rise_advert> rise_adverts { get; set; }
    public DbSet<rise_advertstatistic> rise_advertsstatistics { get; set; }

    public DbSet<rise_project> rise_projects { get; set; }
    public DbSet<rise_unit> rise_units { get; set; }
    public DbSet<rise_competitor> rise_competitors { get; set; }
    public DbSet<rise_competitorphoto> rise_competitorsphotos { get; set; }
    public DbSet<rise_competitorstatistic> rise_competitorsstatistics { get; set; }
    public DbSet<rise_feed> rise_feeds { get; set; }
}