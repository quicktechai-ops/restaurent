export interface ReceiptData {
  orderNumber: string
  orderType: string
  branchName: string
  tableName?: string
  customerName?: string
  lines: {
    name: string
    sizeName?: string
    quantity: number
    effectivePrice: number
    lineNet: number
    discountPercent: number
    modifiers: { name: string; quantity: number; price: number }[]
    notes?: string
  }[]
  subtotal: number
  totalLineDiscount: number
  billDiscountPercent: number
  billDiscountAmount: number
  serviceChargePercent: number
  serviceChargeAmount: number
  vatPercent: number
  vatAmount: number
  grandTotal: number
  paymentMethod: string
  companyName: string
  dateTime: Date
}

function esc(s: string) {
  return s.replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;')
}

function money(n: number) {
  return `$${n.toFixed(2)}`
}

export function printReceipt(data: ReceiptData, t: (key: string) => string) {
  const dateStr = data.dateTime.toLocaleDateString('en-US', { year: 'numeric', month: '2-digit', day: '2-digit' })
  const timeStr = data.dateTime.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', hour12: true })

  let itemsHtml = ''
  data.lines.forEach((line, idx) => {
    const border = idx < data.lines.length - 1 ? 'border-bottom:1px dotted #ddd;' : ''
    let name = esc(line.name)
    if (line.sizeName) name += ` <span style="font-weight:normal;font-size:11px">(${esc(line.sizeName)})</span>`

    let details = ''
    if (line.modifiers.length > 0) {
      const modText = line.modifiers.map(m => `${esc(m.name)}${m.quantity > 1 ? ` x${m.quantity}` : ''}`).join(', ')
      details += `<div style="font-size:10px;color:#555;padding-left:10px;padding-top:2px">+ ${modText}</div>`
    }
    if (line.notes) {
      details += `<div style="font-size:10px;color:#777;padding-left:10px;padding-top:2px;font-style:italic">${esc(line.notes)}</div>`
    }
    if (line.discountPercent > 0) {
      details += `<div style="font-size:10px;color:#c00;padding-left:10px;padding-top:2px">${t('receipt.discount')}: -${line.discountPercent}%</div>`
    }

    itemsHtml += `
      <div style="padding:6px 0;${border}">
        <div style="display:flex;justify-content:space-between;align-items:flex-start">
          <span style="flex:1;font-weight:bold;font-size:13px">${name}</span>
          <span style="width:40px;text-align:center;font-size:12px">${line.quantity}</span>
          <span style="width:60px;text-align:right;font-size:12px">${money(line.effectivePrice)}</span>
          <span style="width:65px;text-align:right;font-weight:bold;font-size:12px">${money(line.lineNet)}</span>
        </div>
        ${details}
      </div>`
  })

  let totalsHtml = `<div style="display:flex;justify-content:space-between;padding:4px 0;font-size:13px"><span>${t('receipt.subtotal')}</span><span>${money(data.subtotal)}</span></div>`

  if (data.totalLineDiscount > 0)
    totalsHtml += `<div style="display:flex;justify-content:space-between;padding:4px 0;font-size:13px;color:#c00"><span>${t('receipt.lineDiscounts')}</span><span>-${money(data.totalLineDiscount)}</span></div>`
  if (data.billDiscountPercent > 0)
    totalsHtml += `<div style="display:flex;justify-content:space-between;padding:4px 0;font-size:13px;color:#c00"><span>${t('receipt.billDiscount')} (${data.billDiscountPercent}%)</span><span>-${money(data.billDiscountAmount)}</span></div>`
  if (data.serviceChargeAmount > 0)
    totalsHtml += `<div style="display:flex;justify-content:space-between;padding:4px 0;font-size:13px"><span>${t('receipt.serviceCharge')} (${data.serviceChargePercent}%)</span><span>${money(data.serviceChargeAmount)}</span></div>`
  if (data.vatAmount > 0)
    totalsHtml += `<div style="display:flex;justify-content:space-between;padding:4px 0;font-size:13px"><span>${t('receipt.vat')} (${data.vatPercent}%)</span><span>${money(data.vatAmount)}</span></div>`

  let metaHtml = ''
  metaHtml += `<div style="display:flex;justify-content:space-between;padding:3px 0;font-size:12px"><span>${t('receipt.orderNo')}</span><span style="font-weight:bold">#${esc(data.orderNumber)}</span></div>`
  metaHtml += `<div style="display:flex;justify-content:space-between;padding:3px 0;font-size:12px"><span>${t('receipt.date')}</span><span>${dateStr} ${timeStr}</span></div>`
  metaHtml += `<div style="display:flex;justify-content:space-between;padding:3px 0;font-size:12px"><span>${t('receipt.type')}</span><span>${esc(data.orderType)}</span></div>`
  if (data.tableName)
    metaHtml += `<div style="display:flex;justify-content:space-between;padding:3px 0;font-size:12px"><span>${t('receipt.table')}</span><span>${esc(data.tableName)}</span></div>`
  if (data.customerName)
    metaHtml += `<div style="display:flex;justify-content:space-between;padding:3px 0;font-size:12px"><span>${t('receipt.customer')}</span><span>${esc(data.customerName)}</span></div>`
  metaHtml += `<div style="display:flex;justify-content:space-between;padding:3px 0;font-size:12px"><span>${t('receipt.payment')}</span><span>${esc(data.paymentMethod)}</span></div>`

  const html = `<!DOCTYPE html><html><head><meta charset="utf-8"/>
    <style>
      * { margin:0; padding:0; box-sizing:border-box; }
      @page { size: 80mm auto; margin: 0; }
      html, body { width:100%; height:auto; margin:0; padding:0; }
      body { font-family:'Segoe UI','Arial','Helvetica Neue',sans-serif; font-size:14px; line-height:1.6; color:#000; background:#fff; padding:6mm 4mm; }
    </style>
  </head><body>
    <div style="font-size:20px;font-weight:bold;text-align:center;margin-bottom:2px">${esc(data.companyName)}</div>
    <div style="font-size:12px;color:#333;text-align:center;margin-bottom:6px">${esc(data.branchName)}</div>
    <hr style="border:none;border-top:1px dashed #000;margin:10px 0"/>
    ${metaHtml}
    <hr style="border:none;border-top:1px dashed #000;margin:10px 0"/>
    <div style="display:flex;justify-content:space-between;font-weight:bold;font-size:11px;padding:4px 0">
      <span style="flex:1">${t('receipt.item')}</span>
      <span style="width:40px;text-align:center">${t('receipt.qty')}</span>
      <span style="width:60px;text-align:right">${t('receipt.price')}</span>
      <span style="width:65px;text-align:right">${t('receipt.total')}</span>
    </div>
    <hr style="border:none;border-top:1px solid #ccc;margin:4px 0"/>
    ${itemsHtml}
    <hr style="border:none;border-top:1px dashed #000;margin:10px 0"/>
    ${totalsHtml}
    <div style="display:flex;justify-content:space-between;font-size:20px;font-weight:bold;padding:8px 0;margin:8px 0;border-top:3px double #000;border-bottom:3px double #000">
      <span>${t('receipt.grandTotal')}</span><span>${money(data.grandTotal)}</span>
    </div>
    <div style="text-align:center;margin-top:14px;padding-top:8px">
      <div style="font-size:13px;font-weight:bold;margin-bottom:4px">${t('receipt.thankYou')}</div>
      <div style="font-size:11px;color:#555">${t('receipt.visitAgain')}</div>
    </div>
  </body></html>`

  // Print via hidden iframe
  const existingFrame = document.getElementById('receipt-print-frame') as HTMLIFrameElement
  if (existingFrame) existingFrame.remove()

  const iframe = document.createElement('iframe')
  iframe.id = 'receipt-print-frame'
  iframe.style.position = 'fixed'
  iframe.style.top = '-10000px'
  iframe.style.left = '-10000px'
  iframe.style.width = '0'
  iframe.style.height = '0'
  iframe.style.border = 'none'
  document.body.appendChild(iframe)

  const doc = iframe.contentDocument || iframe.contentWindow?.document
  if (!doc) return

  doc.open()
  doc.write(html)
  doc.close()

  let printed = false
  const doPrint = () => {
    if (printed) return
    printed = true
    iframe.contentWindow?.print()
    setTimeout(() => iframe.remove(), 1000)
  }

  iframe.onload = () => setTimeout(doPrint, 100)
  setTimeout(doPrint, 600)
}
