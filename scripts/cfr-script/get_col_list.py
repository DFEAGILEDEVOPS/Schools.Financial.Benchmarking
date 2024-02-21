import pandas as pd

df = pd.read_excel('School_total_spend_2021-22_Full_Data_Workbook.xlsx', sheet_name='CFRData', skiprows=2)

# df = pd.read_excel('input.xlsx')

column_names_list = df.columns.tolist()

print("\n".join(column_names_list))
print(len(column_names_list))

# School_total_spend_2021-22_Full_Data_Workbook
# input
