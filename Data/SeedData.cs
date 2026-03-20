using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using library.Models.Entities;
using library.Models.Enums;

namespace library.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            Console.WriteLine("Starting database seeding...");

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // 1. Seed Roles
            await SeedRolesAsync(roleManager);

            // 2. Seed Users
            await SeedUsersAsync(userManager);

            // 3. Seed Authors
            await SeedAuthorsAsync(context);

            // 4. Seed Publishers
            await SeedPublishersAsync(context);

            // 5. Seed Categories (with hierarchy)
            await SeedCategoriesAsync(context);

            // 6. Seed Books
            await SeedBooksAsync(context);

            // 7. Seed Editions and Items
            await SeedEditionsAndItemsAsync(context);

            Console.WriteLine("✅ Database seeding completed!");
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "USER", "ADMIN" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    Console.WriteLine($"  ✓ Role created: {role}");
                }
            }
        }

        private static async Task SeedUsersAsync(UserManager<User> userManager)
        {
            // Admin user - gets ADMIN role
            var adminEmail = "admin@library.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    MemberSince = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    // ✅ Use "ADMIN" (matches your seeded role)
                    await userManager.AddToRoleAsync(admin, "ADMIN");
                    Console.WriteLine("  ✓ Admin user created");
                }
            }

            // Librarian user - you could give them ADMIN role for now, 
            // or create them as a regular USER until you add LIBRARIAN role later
            var librarianEmail = "librarian@library.com";
            if (await userManager.FindByEmailAsync(librarianEmail) == null)
            {
                var librarian = new User
                {
                    UserName = librarianEmail,
                    Email = librarianEmail,
                    FirstName = "Jane",
                    LastName = "Smith",
                    MemberSince = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(librarian, "Librarian@123");
                if (result.Succeeded)
                {
                    // ✅ Option 1: Give ADMIN role (if librarians should have admin access)
                    await userManager.AddToRoleAsync(librarian, "ADMIN");

                    // ✅ Option 2: Give USER role (if librarians are just regular users for now)
                    // await userManager.AddToRoleAsync(librarian, "USER");

                    Console.WriteLine("  ✓ Librarian user created");
                }
            }

            // Member user - give them USER role
            var memberEmail = "member@library.com";
            if (await userManager.FindByEmailAsync(memberEmail) == null)
            {
                var member = new User
                {
                    UserName = memberEmail,
                    Email = memberEmail,
                    FirstName = "John",
                    LastName = "Doe",
                    MemberSince = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(member, "Member@123");
                if (result.Succeeded)
                {
                    // ✅ Use "USER" (matches your seeded role)
                    await userManager.AddToRoleAsync(member, "USER");
                    Console.WriteLine("  ✓ Member user created");
                }
            }
        }

        private static async Task SeedAuthorsAsync(AppDbContext context)
        {
            if (await context.Authors.AnyAsync()) return;

            var authors = new[]
            {
                new Author { FullName = "J.R.R. Tolkien", Biography = "Creator of Middle-earth", CreatedAt = DateTime.UtcNow },
                new Author { FullName = "George R.R. Martin", Biography = "Author of A Song of Ice and Fire", CreatedAt = DateTime.UtcNow },
                new Author { FullName = "J.K. Rowling", Biography = "Creator of Harry Potter", CreatedAt = DateTime.UtcNow },
                new Author { FullName = "Isaac Asimov", Biography = "Science fiction writer", CreatedAt = DateTime.UtcNow },
                new Author { FullName = "Agatha Christie", Biography = "Mystery writer", CreatedAt = DateTime.UtcNow },
                new Author { FullName = "Stephen King", Biography = "Horror and suspense author", CreatedAt = DateTime.UtcNow },
                new Author { FullName = "Jane Austen", Biography = "English novelist", CreatedAt = DateTime.UtcNow },
                new Author { FullName = "Ernest Hemingway", Biography = "American novelist", CreatedAt = DateTime.UtcNow }
            };

            await context.Authors.AddRangeAsync(authors);
            await context.SaveChangesAsync();
            Console.WriteLine($"  ✓ {authors.Length} authors added");
        }

        private static async Task SeedPublishersAsync(AppDbContext context)
        {
            if (await context.Publishers.AnyAsync()) return;

            var publishers = new[]
            {
                new Publisher { Name = "HarperCollins", Website = "https://www.harpercollins.com", CreatedAt = DateTime.UtcNow },
                new Publisher { Name = "Penguin Random House", Website = "https://www.penguinrandomhouse.com", CreatedAt = DateTime.UtcNow },
                new Publisher { Name = "Simon & Schuster", Website = "https://www.simonandschuster.com", CreatedAt = DateTime.UtcNow },
                new Publisher { Name = "Macmillan Publishers", Website = "https://www.macmillan.com", CreatedAt = DateTime.UtcNow },
                new Publisher { Name = "Hachette Book Group", Website = "https://www.hachette.com", CreatedAt = DateTime.UtcNow }
            };

            await context.Publishers.AddRangeAsync(publishers);
            await context.SaveChangesAsync();
            Console.WriteLine($"  ✓ {publishers.Length} publishers added");
        }

        private static async Task SeedCategoriesAsync(AppDbContext context)
        {
            if (await context.Categories.AnyAsync()) return;

            // Main categories
            var fiction = new Category { Name = "Fiction", SortOrder = 1, CreatedAt = DateTime.UtcNow };
            var nonFiction = new Category { Name = "Non-Fiction", SortOrder = 2, CreatedAt = DateTime.UtcNow };
            var science = new Category { Name = "Science", SortOrder = 3, CreatedAt = DateTime.UtcNow };

            // Subcategories of Fiction
            var fantasy = new Category { Name = "Fantasy", ParentCategory = fiction, SortOrder = 1, CreatedAt = DateTime.UtcNow };
            var scienceFiction = new Category { Name = "Science Fiction", ParentCategory = fiction, SortOrder = 2, CreatedAt = DateTime.UtcNow };
            var mystery = new Category { Name = "Mystery", ParentCategory = fiction, SortOrder = 3, CreatedAt = DateTime.UtcNow };
            var romance = new Category { Name = "Romance", ParentCategory = fiction, SortOrder = 4, CreatedAt = DateTime.UtcNow };
            var horror = new Category { Name = "Horror", ParentCategory = fiction, SortOrder = 5, CreatedAt = DateTime.UtcNow };

            // Subcategories of Non-Fiction
            var biography = new Category { Name = "Biography", ParentCategory = nonFiction, SortOrder = 1, CreatedAt = DateTime.UtcNow };
            var history = new Category { Name = "History", ParentCategory = nonFiction, SortOrder = 2, CreatedAt = DateTime.UtcNow };
            var philosophy = new Category { Name = "Philosophy", ParentCategory = nonFiction, SortOrder = 3, CreatedAt = DateTime.UtcNow };

            // Subcategories of Science
            var physics = new Category { Name = "Physics", ParentCategory = science, SortOrder = 1, CreatedAt = DateTime.UtcNow };
            var chemistry = new Category { Name = "Chemistry", ParentCategory = science, SortOrder = 2, CreatedAt = DateTime.UtcNow };
            var biology = new Category { Name = "Biology", ParentCategory = science, SortOrder = 3, CreatedAt = DateTime.UtcNow };
            var computerScience = new Category { Name = "Computer Science", ParentCategory = science, SortOrder = 4, CreatedAt = DateTime.UtcNow };

            var categories = new[]
            {
                fiction, nonFiction, science,
                fantasy, scienceFiction, mystery, romance, horror,
                biography, history, philosophy,
                physics, chemistry, biology, computerScience
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
            Console.WriteLine($"  ✓ {categories.Length} categories added with hierarchy");
        }

        private static async Task SeedBooksAsync(AppDbContext context)
        {
            if (await context.Books.AnyAsync()) return;

            var authors = await context.Authors.ToListAsync();
            var publishers = await context.Publishers.ToListAsync();
            var categories = await context.Categories.ToListAsync();

            var books = new List<Book>();

            // Helper to get category by name
            Category? GetCategory(string name) =>
                categories.FirstOrDefault(c => c.Name == name);

            // 1. The Hobbit
            var book1 = new Book
            {
                Title = "The Hobbit",
                Description = "A fantasy novel about Bilbo Baggins who embarks on an epic quest",
                PublisherId = publishers.First(p => p.Name.Contains("HarperCollins")).Id,
                CreatedAt = DateTime.UtcNow,
                BookAuthors = new List<BookAuthor>(),
                BookCategories = new List<BookCategory>()
            };
            book1.BookAuthors.Add(new BookAuthor
            {
                Author = authors.First(a => a.FullName.Contains("Tolkien")),
                AuthorOrder = 1
            });
            book1.BookCategories.Add(new BookCategory
            {
                Category = GetCategory("Fantasy")!
            });
            books.Add(book1);

            // 2. Foundation
            var book2 = new Book
            {
                Title = "Foundation",
                Description = "A science fiction novel about the decline and fall of a galactic empire",
                PublisherId = publishers.First(p => p.Name.Contains("Penguin")).Id,
                CreatedAt = DateTime.UtcNow,
                BookAuthors = new List<BookAuthor>(),
                BookCategories = new List<BookCategory>()
            };
            book2.BookAuthors.Add(new BookAuthor
            {
                Author = authors.First(a => a.FullName.Contains("Asimov")),
                AuthorOrder = 1
            });
            book2.BookCategories.Add(new BookCategory
            {
                Category = GetCategory("Science Fiction")!
            });
            books.Add(book2);

            // 3. Harry Potter
            var book3 = new Book
            {
                Title = "Harry Potter and the Philosopher's Stone",
                Description = "A young wizard's first year at Hogwarts School of Witchcraft and Wizardry",
                PublisherId = publishers.First(p => p.Name.Contains("Penguin")).Id,
                CreatedAt = DateTime.UtcNow,
                BookAuthors = new List<BookAuthor>(),
                BookCategories = new List<BookCategory>()
            };
            book3.BookAuthors.Add(new BookAuthor
            {
                Author = authors.First(a => a.FullName.Contains("Rowling")),
                AuthorOrder = 1
            });
            book3.BookCategories.Add(new BookCategory
            {
                Category = GetCategory("Fantasy")!
            });
            books.Add(book3);

            // 4. A Game of Thrones
            var book4 = new Book
            {
                Title = "A Game of Thrones",
                Description = "The first book in the epic fantasy series A Song of Ice and Fire",
                PublisherId = publishers.First(p => p.Name.Contains("HarperCollins")).Id,
                CreatedAt = DateTime.UtcNow,
                BookAuthors = new List<BookAuthor>(),
                BookCategories = new List<BookCategory>()
            };
            book4.BookAuthors.Add(new BookAuthor
            {
                Author = authors.First(a => a.FullName.Contains("Martin")),
                AuthorOrder = 1
            });
            book4.BookCategories.Add(new BookCategory
            {
                Category = GetCategory("Fantasy")!
            });
            books.Add(book4);

            // 5. Murder on the Orient Express
            var book5 = new Book
            {
                Title = "Murder on the Orient Express",
                Description = "Famous detective Hercule Poirot investigates a murder on a train",
                PublisherId = publishers.First(p => p.Name.Contains("Penguin")).Id,
                CreatedAt = DateTime.UtcNow,
                BookAuthors = new List<BookAuthor>(),
                BookCategories = new List<BookCategory>()
            };
            book5.BookAuthors.Add(new BookAuthor
            {
                Author = authors.First(a => a.FullName.Contains("Christie")),
                AuthorOrder = 1
            });
            book5.BookCategories.Add(new BookCategory
            {
                Category = GetCategory("Mystery")!
            });
            books.Add(book5);

            // 6. The Shining
            var book6 = new Book
            {
                Title = "The Shining",
                Description = "A family becomes caretakers of an isolated hotel where dark forces exist",
                PublisherId = publishers.First(p => p.Name.Contains("Simon")).Id,
                CreatedAt = DateTime.UtcNow,
                BookAuthors = new List<BookAuthor>(),
                BookCategories = new List<BookCategory>()
            };
            book6.BookAuthors.Add(new BookAuthor
            {
                Author = authors.First(a => a.FullName.Contains("King")),
                AuthorOrder = 1
            });
            book6.BookCategories.Add(new BookCategory
            {
                Category = GetCategory("Horror")!
            });
            books.Add(book6);

            // 7. Pride and Prejudice
            var book7 = new Book
            {
                Title = "Pride and Prejudice",
                Description = "A classic romance novel about Elizabeth Bennet and Mr. Darcy",
                PublisherId = publishers.First(p => p.Name.Contains("Penguin")).Id,
                CreatedAt = DateTime.UtcNow,
                BookAuthors = new List<BookAuthor>(),
                BookCategories = new List<BookCategory>()
            };
            book7.BookAuthors.Add(new BookAuthor
            {
                Author = authors.First(a => a.FullName.Contains("Austen")),
                AuthorOrder = 1
            });
            book7.BookCategories.Add(new BookCategory
            {
                Category = GetCategory("Romance")!
            });
            books.Add(book7);

            // 8. A Brief History of Time
            var book8 = new Book
            {
                Title = "A Brief History of Time",
                Description = "A popular science book about cosmology by Stephen Hawking",
                PublisherId = publishers.First(p => p.Name.Contains("Penguin")).Id,
                CreatedAt = DateTime.UtcNow,
                BookAuthors = new List<BookAuthor>(),
                BookCategories = new List<BookCategory>()
            };

            // Create Stephen Hawking if not exists
            var hawking = authors.FirstOrDefault(a => a.FullName.Contains("Hawking"));
            if (hawking == null)
            {
                hawking = new Author
                {
                    FullName = "Stephen Hawking",
                    Biography = "Theoretical physicist and cosmologist",
                    CreatedAt = DateTime.UtcNow
                };
                await context.Authors.AddAsync(hawking);
                await context.SaveChangesAsync();
            }

            book8.BookAuthors.Add(new BookAuthor
            {
                Author = hawking,
                AuthorOrder = 1
            });
            book8.BookCategories.Add(new BookCategory
            {
                Category = GetCategory("Science")!
            });
            book8.BookCategories.Add(new BookCategory
            {
                Category = GetCategory("Physics")!
            });
            books.Add(book8);

            await context.Books.AddRangeAsync(books);
            await context.SaveChangesAsync();
            Console.WriteLine($"  ✓ {books.Count} books added with authors and categories");
        }

        private static async Task SeedEditionsAndItemsAsync(AppDbContext context)
        {
            if (await context.Editions.AnyAsync()) return;

            var books = await context.Books.ToListAsync();
            var random = new Random();
            var itemsAdded = 0;
            var usedBarcodes = new HashSet<string>(); // Track used barcodes

            foreach (var book in books)
            {
                // Add 2-3 editions per book
                int editionCount = random.Next(2, 4);
                for (int e = 1; e <= editionCount; e++)
                {
                    var edition = new Edition
                    {
                        BookId = book.Id,
                        ISBN = $"978-{random.Next(100000000, 999999999)}",
                        Language = "English",
                        Format = e == 1 ? "Hardcover" : (e == 2 ? "Paperback" : "eBook"),
                        PublicationYear = 2020 - e,
                        PageCount = 300 + (e * 50),
                        CreatedAt = DateTime.UtcNow
                    };

                    await context.Editions.AddAsync(edition);
                    await context.SaveChangesAsync();

                    // Add 2-5 copies per edition
                    int copyCount = random.Next(2, 6);
                    for (int i = 1; i <= copyCount; i++)
                    {
                        var status = i <= copyCount - 1 ? ItemStatus.Available : ItemStatus.Borrowed;

                        // Generate UNIQUE barcode
                        string baseBarcode = $"{book.Title.Substring(0, Math.Min(3, book.Title.Length)).ToUpper()}{e:D2}{i:D3}";
                        string barcode = baseBarcode;
                        int counter = 1;

                        // Ensure uniqueness
                        while (usedBarcodes.Contains(barcode))
                        {
                            barcode = $"{baseBarcode}{counter++}";
                        }
                        usedBarcodes.Add(barcode);

                        var item = new Item
                        {
                            EditionId = edition.Id,
                            Barcode = barcode,
                            AcquisitionDate = DateTime.UtcNow.AddDays(-random.Next(1, 365)),
                            Price = 19.99m + (e * 5),
                            ItemStatus = status,
                            Location = $"Shelf {book.Title.Substring(0, 1).ToUpper()}-{e}-{i}",
                            CreatedAt = DateTime.UtcNow
                        };
                        await context.Items.AddAsync(item);
                        itemsAdded++;
                    }
                }
            }

            await context.SaveChangesAsync();
            Console.WriteLine($"  ✓ Editions added with {itemsAdded} physical items");
        }
    }
}