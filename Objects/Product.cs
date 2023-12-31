﻿namespace OnlineShopPoC.Objects
{
    /// <summary>
    /// Represents a product in an online shop.
    /// </summary>
    public class Product : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class with the specified name and price.
        /// </summary>
        /// <param name="name">The name of the product.</param>
        /// <param name="price">The price of the product.</param>
        public Product(string name, decimal price)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Id = Guid.NewGuid();
            Name = name;
            Price = price;
        }

        /// <summary> Product's Id. </summary>
        public Guid Id { get; set; }


        /// <summary> Product's Name. </summary>
        public string Name { get; set; }


        /// <summary> Product's description. </summary>
        public string? Description { get; set; }



        /// <summary> Product's price. </summary>
        public decimal Price { get; set; }


        /// <summary> Product's production date. </summary>
        public DateTime ProducedAt { get; set; }


        /// <summary> Product's expiry date. </summary>
        public DateTime ExpiredAt { get; set; }


        /// <summary> Product's quantity in stock. </summary>
        public double Stock { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}