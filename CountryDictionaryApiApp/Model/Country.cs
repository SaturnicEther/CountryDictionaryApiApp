namespace CountryDictionaryApiApp.Model
{
    public class Country
    {
        public int Id { get; set; }                     // идентификатор записи БД
        public string Name { get; set; }                // наименование страны
        public string ISO31661Alpha2Code { get; set; }  // двухбуквенный код страны
        public string ISO31661Alpha3Code { get; set; }  // трехбуквенный код страны
        public string ISO31661NumericCode { get; set; } // числовой код страны

        public Country() : this(0, string.Empty, string.Empty, string.Empty, string.Empty) { }

        public Country(int id, string name, string iso31661Alpha2Code, string iso31661Alpha3Code, string iso31661NumericCode)
        {
            Id = id;
            Name = name;
            ISO31661Alpha2Code = iso31661Alpha2Code;
            ISO31661Alpha3Code = iso31661Alpha3Code;
            ISO31661NumericCode = iso31661NumericCode;
        }

        public override string ToString()
        {
            return $"{Id} - {Name}; " +
                $"ISO 3166-1 alpha-2: {ISO31661Alpha2Code}; " +
                $"ISO 3166-1 alpha-3: {ISO31661Alpha3Code}; " +
                $"ISO 3166-1 numeric: {ISO31661NumericCode}";
        }
    }

}
