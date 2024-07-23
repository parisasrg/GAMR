import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from scipy.ndimage import gaussian_filter

df1 = pd.read_csv("C:/Users/psarg/Desktop/data.csv")
df2 = pd.read_csv("C:/Users/psarg/Desktop/data2.csv")
# df3 = pd.read_csv("C:/Users/psarg/Desktop/data2.csv")
# df4 = pd.read_csv("C:/Users/psarg/Desktop/data.csv")
# df5 = pd.read_csv("C:/Users/psarg/Desktop/data.csv")
# df6 = pd.read_csv("C:/Users/psarg/Desktop/data2.csv")
# df7 = pd.read_csv("C:/Users/psarg/Desktop/data.csv")
# df8 = pd.read_csv("C:/Users/psarg/Desktop/data2.csv")
# df9 = pd.read_csv("C:/Users/psarg/Desktop/data.csv")
# df10 = pd.read_csv("C:/Users/psarg/Desktop/data2.csv")
# df11 = pd.read_csv("C:/Users/psarg/Desktop/data.csv")
# df12 = pd.read_csv("C:/Users/psarg/Desktop/data2.csv")
# df13 = pd.read_csv("C:/Users/psarg/Desktop/data2.csv")
# df14 = pd.read_csv("C:/Users/psarg/Desktop/data.csv")
# df15 = pd.read_csv("C:/Users/psarg/Desktop/data.csv")
# df16 = pd.read_csv("C:/Users/psarg/Desktop/data2.csv")
# df17 = pd.read_csv("C:/Users/psarg/Desktop/data.csv")
# df18 = pd.read_csv("C:/Users/psarg/Desktop/data2.csv")

frames = [df1, df2]

df = pd.concat(frames)

def myplot(x, y, s, bins=1000):
    heatmap, xedges, yedges = np.histogram2d(x, y, bins=bins)
    heatmap = gaussian_filter(heatmap, sigma=s)

    return heatmap.T



# fig, axs = plt.subplots(2, 2)

# Generate some test data
x = df[df.columns[0]]
y = df[df.columns[1]]

# sigmas = [0, 16, 32, 64]
#
# for ax, s in zip(axs.flatten(), sigmas):
#     if s == 0:
#         ax.plot(x, y, 'k.', markersize=5)
#     else:
#         img= myplot(x, y, s)
#         ax.imshow(img, origin='lower', cmap=cm.jet)

img= myplot(x, y, 64)
plt.imshow(img, origin='lower', cmap="jet")
plt.axis('off')

# plt.show()
plt.savefig('C:/Users/psarg/Desktop/foo.png', bbox_inches='tight', pad_inches = 0)