using ZXing;
using ZXing.Common;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CinephoriaServer.API.Models.PostgresqlDb;

namespace CinephoriaServer.API.Services
{
    public class QRCodeService
    {
        /// <summary>
        /// Génère un QRCode pour une réservation spécifique.
        /// </summary>
        /// <param name="reservation">La réservation pour laquelle générer le QRCode.</param>
        /// <returns>Le QRCode sous forme de tableau de bytes (image).</returns>
        public byte[] GenerateQRCode(Models.PostgresqlDb.Reservation reservation)
        {
            // Vérification des entrées
            if (reservation == null)
            {
                throw new ArgumentNullException(nameof(reservation), "La réservation ne peut pas être nulle.");
            }

            if (reservation.ReservationId == 0 || reservation.ShowtimeId == 0 || string.IsNullOrEmpty(reservation.AppUserId))
            {
                throw new ArgumentException("Les champs ReservationId, ShowtimeId et AppUserId doivent être valides.");
            }

            // Génération du contenu du QRCode
            string qrCodeData = $"{reservation.ReservationId}-{reservation.ShowtimeId}-{reservation.AppUserId}";

            // Configuration du générateur de QRCode
            var barcodeWriter = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Width = 200,
                    Height = 200,
                    Margin = 1
                }
            };

            // Génération des pixels du QRCode
            var pixelData = barcodeWriter.Write(qrCodeData);

            if (pixelData == null || pixelData.Pixels == null || pixelData.Pixels.Length == 0)
            {
                throw new InvalidOperationException("La génération des données du QRCode a échoué.");
            }

            // Conversion des pixels en image Bitmap
            using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb))
            {
                var bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.WriteOnly,
                    bitmap.PixelFormat
                );

                try
                {
                    // Copie des pixels dans l'image Bitmap
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }

                // Conversion de l'image en tableau de bytes (format PNG)
                using (var memoryStream = new MemoryStream())
                {
                    bitmap.Save(memoryStream, ImageFormat.Png); // Utilisation du format PNG
                    return memoryStream.ToArray();
                }
            }
        }
    }
}