namespace CodeCityCrew.Settings.Abstractions
{
    /// <summary>
    /// Provides a mechanism to manage settings.
    /// </summary>
    public interface ISettingService
    {
        /// <summary>
        /// Gets the setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T As<T>() where T : new();

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        void Save<T>(T value);
    }
}
