namespace CodeCityCrew.Settings.Abstractions
{
    /// <summary>
    /// Provides a mechanism to manage settings.
    /// </summary>
    public interface ISettingService
    {
        /// <summary>
        /// Access the specified setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="forceReload">if set to <c>true</c> [force reload].</param>
        /// <returns></returns>
        T As<T>(bool forceReload = false) where T : new();

        /// <summary>
        /// Gets object by full name.
        /// </summary>
        /// <param name="id">The full name.</param>
        /// <returns></returns>
        object As(string id);

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        void Save<T>(T value);
    }
}
