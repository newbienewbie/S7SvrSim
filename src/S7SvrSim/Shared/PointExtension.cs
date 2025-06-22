using System.Windows;

namespace S7SvrSim.Shared
{
    public static class PointExtension
    {
        /// <summary>
        /// <para>From: <seealso href="https://stackoverflow.com/questions/4226740/how-do-i-get-the-current-mouse-screen-coordinates-in-wpf"/></para>
        /// <para>Answer: <seealso href="https://stackoverflow.com/a/19790851"/></para>
        /// </summary>
        /// <param name="point"></param>
        /// <param name="visual"></param>
        /// <returns></returns>
        public static Point ToWinPoint(this System.Drawing.Point point, System.Windows.Media.Visual visual)
        {
            var transform = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
            return transform.Transform(new Point(point.X, point.Y));
        }
    }
}
