using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReadingIsGood.Application.Models;
using ReadingIsGood.Application.Services.Books;
using ReadingIsGood.Application.Services.Orders;
using ReadingIsGood.Domain.Constants;
using ReadingIsGood.Domain.Entities;
using ReadingIsGood.Infrastructure.Context;

namespace ReadingIsGood.Infrastructure.DbInitializer;

public class DbInitializer(
    UserManager<Customer> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    IBookService bookService,
    IOrderService orderService,
    ApplicationDbContext db) : IDbInitializer
{
    public async Task Initialize()
    {
        try
        {
            if ((await db.Database.GetPendingMigrationsAsync()).Any())
            {
                await db.Database.MigrateAsync();
            }

            await SeedRoles();
            await SeedSampleData();
        }
        catch (Exception e)
        {
            // ignored
        }
    }

    private async Task SeedRoles()
    {
        var roles = new List<string>
        {
            Roles.Admin,
            Roles.Customer
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }

    private async Task SeedSampleData()
    {
        var adminUser = new Customer
        {
            UserName = "admin@admin.com",
            Email = "admin@admin.com",
            Firstname = "Admin",
            Lastname = "Admin",
            CountryCode = "+90",
            PhoneNumber = "5370000000",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            Address = "Admin Address",
            IsActive = true,
            IsDeleted = false
        };

        await userManager.CreateAsync(adminUser, "Ab123456**");

        var admin = await userManager.Users.FirstOrDefaultAsync(x => x.Email == adminUser.Email) ?? throw new Exception("Admin user not found");
        await userManager.AddToRoleAsync(admin, Roles.Admin);

        var customerUser = new Customer
        {
            UserName = "customer@customer.com",
            Email = "customer@customer.com",
            Firstname = "John",
            Lastname = "Doe",
            CountryCode = "+90",
            PhoneNumber = "5321111111",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            Address = "Customer Address",
            IsActive = true,
            IsDeleted = false
        };

        await userManager.CreateAsync(customerUser, "Ab123456**");

        var customer = await userManager.Users.FirstOrDefaultAsync(x => x.Email == customerUser.Email) ?? throw new Exception("Admin user not found");
        await userManager.AddToRoleAsync(customer, Roles.Customer);

        var books = new List<BookModel>
        {
            new()
            {
                Title = "The Name of the Wind",
                Author = "Patrick Rothfuss",
                ISBN = "9780575087057",
                ImageUrl = "https://books.google.com/books/content?id=BcG2dVRXKukC&printsec=frontcover&img=1&zoom=5&source=gbs_api",
                Description = "'I have stolen princesses back from sleeping barrow kings. I burned down the town of Trebon. I have spent the night with Felurian and left with both my sanity and my life. I was expelled from the University at a younger age than most people are allowed in. I tread paths by moonlight that others fear to speak of during day. I have talked to Gods, loved women, and written songs that make the minstrels weep. My name is Kvothe. You may have heard of me' So begins the tale of Kvothe - currently known as Kote, the unassuming innkeepter - from his childhood in a troupe of traveling players, through his years spent as a near-feral orphan in a crime-riddled city, to his daringly brazen yet successful bid to enter a difficult and dangerous school of magic. In these pages you will come to know Kvothe the notorious magician, the accomplished thief, the masterful musician, the dragon-slayer, the legend-hunter, the lover, the thief and the infamous assassin. The Name of the Wind is fantasy at its very best, and an astounding must-read title.",
                Price = 9.99m,
                StockQuantity = 10
            },
            new()
            {
                Title = "Dune",
                Author = "Frank Herbert",
                ISBN = "9780441013593",
                ImageUrl = "http://books.google.com/books/content?id=B1hSG45JCX4C&printsec=frontcover&img=1&zoom=5&edge=curl&source=gbs_api",
                Description = "Follows the adventures of Paul Atreides, the son of a betrayed duke given up for dead on a treacherous desert planet and adopted by its fierce, nomadic people, who help him unravel his most unexpected destiny.",
                Price = 8.89m,
                StockQuantity = 10
            },
            new()
            {
                Title = "The Left Hand of Darkness",
                Author = "Ursula K. Le Guid",
                ISBN = "9780143111597",
                ImageUrl = "http://books.google.com/books/content?id=f9QiDQAAQBAJ&printsec=frontcover&img=1&zoom=5&edge=curl&source=gbs_api",
                Description = "A deluxe hardcover edition of the queen of science fiction's trailblazing novel about a planet full of genderless beings--part of Penguin Galaxy, a collectible series of six sci-fi/fantasy classics, featuring a series introduction by Neil Gaiman A groundbreaking work of science fiction, The Left Hand of Darkness tells the story of a lone human emissary's mission to Winter, an unknown alien world whose inhabitants can choose--and change--their gender. His goal is to facilitate Winter's inclusion in a growing intergalactic civilization. But to do so he must bridge the gulf between his own views and those of the completely dissimilar culture that he encounters. Exploring questions of psychology, society, and human emotion in an alien world, The Left Hand of Darkness stands as a landmark achievement in the annals of science fiction. Penguin Galaxy Six of our greatest masterworks of science fiction and fantasy, in dazzling collector-worthy hardcover editions, and featuring a series introduction by #1 New York Times bestselling author Neil Gaiman, Penguin Galaxy represents a constellation of achievement in visionary fiction, lighting the way toward our knowledge of the universe, and of ourselves. From historical legends to mythic futures, monuments of world-building to mind-bending dystopias, these touchstones of human invention and storytelling ingenuity have transported millions of readers to distant realms, and will continue for generations to chart the frontiers of the imagination. The Once and Future King by T. H. White Stranger in a Strange Land by Robert A. Heinlein Dune by Frank Herbert 2001: A Space Odyssey by Arthur C. Clarke The Left Hand of Darkness by Ursula K. Le Guin Neuromancer by William Gibson For more than seventy years, Penguin has been the leading publisher of classic literature in the English-speaking world. With more than 1,700 titles, Penguin Classics represents a global bookshelf of the best works throughout history and across genres and disciplines. Readers trust the series to provide authoritative texts enhanced by introductions and notes by distinguished scholars and contemporary authors, as well as up-to-date translations by award-winning translators.",
                Price = 8.89m,
                StockQuantity = 10
            },
            new()
            {
                Title = "Foundation",
                Author = "Isaac Asimov",
                ISBN = "9780553900347",
                ImageUrl = "http://books.google.com/books/content?id=IwywDY4P6gsC&printsec=frontcover&img=1&zoom=5&edge=curl&source=gbs_api",
                Description = "For twelve thousand years the Galactic Empire has ruled supreme. Now it is dying. But only Hari Sheldon, creator of the revolutionary science of psychohistory, can see into the future--to a dark age of ignorance, barbarism, and warfare that will last thirty thousand years. To preserve knowledge and save mankind, Seldon gathers the best minds in the Empire--both scientists and scholars--and brings them to a bleak planet at the edge of the Galaxy to serve as a beacon of hope for a fututre generations. He calls his sanctuary the Foundation. But soon the fledgling Foundation finds itself at the mercy of corrupt warlords rising in the wake of the receding Empire. Mankind's last best hope is faced with an agonizing choice: submit to the barbarians and be overrun--or fight them and be destroyed.",
                Price = 8.89m,
                StockQuantity = 10
            },
            new()
            {
                Title = "Stranger in a Strange Land",
                Author = "Robert A Heinlein",
                ISBN = "9780143111627",
                ImageUrl = "http://books.google.com/books/content?id=p9UiDQAAQBAJ&printsec=frontcover&img=1&zoom=5&edge=curl&source=gbs_api",
                Description = "A deluxe hardcover edition of the most famous science-fiction novel of all time--part of Penguin Galaxy, a collectible series of six sci-fi/fantasy classics, featuring a series introduction by Neil Gaiman A human raised on Mars, Valentine Michael Smith has just arrived on planet Earth. Among his people for the first time, he struggles to understand the social mores and prejudices of human nature that are so alien to him, while his own \\\"psi\\\" powers--including telepathy, clairvoyance, telekenesis, and teleportation--make him a type of messiah figure among humans. Stranger in a Strange Land grew from a cult favorite to a bestseller to a classic in a few short years. The story of the man from Mars who taught humankind grokking and water-sharing--and love--it is Robert A. Heinlein's masterpiece. Penguin Galaxy Six of our greatest masterworks of science fiction and fantasy, in dazzling collector-worthy hardcover editions, and featuring a series introduction by #1 New York Times bestselling author Neil Gaiman, Penguin Galaxy represents a constellation of achievement in visionary fiction, lighting the way toward our knowledge of the universe, and of ourselves. From historical legends to mythic futures, monuments of world-building to mind-bending dystopias, these touchstones of human invention and storytelling ingenuity have transported millions of readers to distant realms, and will continue for generations to chart the frontiers of the imagination. The Once and Future King by T. H. White Stranger in a Strange Land by Robert A. Heinlein Dune by Frank Herbert 2001: A Space Odyssey by Arthur C. Clarke The Left Hand of Darkness by Ursula K. Le Guin Neuromancer by William Gibson For more than seventy years, Penguin has been the leading publisher of classic literature in the English-speaking world. With more than 1,700 titles, Penguin Classics represents a global bookshelf of the best works throughout history and across genres and disciplines. Readers trust the series to provide authoritative texts enhanced by introductions and notes by distinguished scholars and contemporary authors, as well as up-to-date translations by award-winning translators.",
                Price = 8.89m,
                StockQuantity = 10
            }
        };

        foreach (var book in books)
        {
            await bookService.CreateBookAsync(book);
        }

        var allBooks = await bookService.GetBookListAsync(1, 1000);

        for (var i = 0; i < 10; i++)
        {
            var rng = new Random();
            var randomNumber = rng.Next(1, 9);
            await orderService.CreateOrderAsync(customerUser.Id, allBooks.Item1[i % allBooks.Item1.Count].Id, randomNumber);
        }
    }
}