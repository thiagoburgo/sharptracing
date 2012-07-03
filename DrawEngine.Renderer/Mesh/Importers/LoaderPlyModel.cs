/**
 * Criado por: Thiago Burgo Belo (thiagoburgo@gmail.com)
 * SharpTracing é um projeto feito inicialmente para disciplina
 * Computação Gráfica da UFPE e depois melhorado nos tempos livres
 * sinta-se a vontade para copiar modificar e mandar correções e 
 * sugestões. Mantenha os créditos!
 * **************************************************************
 * SharpTracing is a project originally created to discipline 
 * Computer Graphics of UFPE and was improved in my free time.
 * Feel free to copy, modify and  give fixes 
 * suggestions. Keep the credits!
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DrawEngine.Renderer.Mathematics.Algebra;
using DrawEngine.Renderer.RenderObjects;
using Util.IO;
using Util.Conversion;
using System.Linq;
using DrawEngine.Renderer.Util.IO;
using System.Text;

namespace DrawEngine.Renderer.Importers
{
	public enum ElementMesh
	{
		Vertex,
		Triangle,
		VertexNormal
	}
	public enum PlyFormat
	{
		ascii,
		binary_big_endian,
		binary_little_endian
	}
	public enum PropertyType
	{
		@char,
		uchar,
		@short,
		@ushort,
		@int,
		@uint,
		@float,
		@double,

		int8,
		uint8,
		int16,
		uint16,
		int32,
		uint32,
		float32,
		float64,
		list
	}

	public class NamedList<T> : List<T> where T : IName
	{
		public NamedList() : base() { }
		public NamedList(int capacity) : base(capacity) { }
		public NamedList(IEnumerable<T> collection) : base(collection) { }
		public T this[string name]
		{
			get { return (from element in this where element.Name == name select element).First(); }
		}
	}
	public interface IName
	{
		String Name { get; set; }
	}
	public class ElementDescription : IName
	{
		public ElementDescription(String name, int count)
		{
			this.Name = name;
			this.Count = count;
			this.Properties = new PropertyDescriptionList();
		}
		public string Name { get; set; }
		public int Count { get; set; }
		public PropertyDescriptionList Properties { get; set; }
	}
	public class PropertyDescriptionList : NamedList<PropertyDescription>
	{
		public PropertyDescriptionList()
			: base()
		{
		}
		public PropertyDescriptionList(int capacity)
			: base(capacity)
		{
		}
		public PropertyDescriptionList(IEnumerable<PropertyDescription> collection)
			: base(collection)
		{
		}
		/// <summary>
		/// Sum of Types Length in bytes
		/// </summary>
		public int SumTypesLength
		{
			get{
				int lengthTypes = 0;
				foreach (PropertyDescription p in this)
				{
					lengthTypes += p.TypeLength;
				}
				return lengthTypes;
			}
		}
	}
	public class PropertyDescription : IName
	{
		public PropertyDescription(PropertyType type, String name)
		{
			this.Type = type;
			this.Name = name;
			this.TypeLength = GetTypeLength(type);
		}
		private static int GetTypeLength(PropertyType type)
		{
			switch (type)
			{
				case PropertyType.@char:
				case PropertyType.int8:
				case PropertyType.uchar:
				case PropertyType.uint8:
					return 1;
				case PropertyType.@short:
				case PropertyType.int16:
				case PropertyType.@ushort:
				case PropertyType.uint16:
					return 2;
				case PropertyType.@int:
				case PropertyType.int32:
				case PropertyType.@uint:
				case PropertyType.uint32:
				case PropertyType.@float:
				case PropertyType.float32:
					return 4;
				case PropertyType.@double:
				case PropertyType.float64:
					return 8;
			}
			return 0;
		}
		/// <summary>
		/// Type Length in bytes
		/// </summary>
		public int TypeLength { get; private set; }
		public PropertyType Type { get; set; }
		public string Name { get; set; }
		/// <summary>
		/// Only if Type is a list
		/// </summary>
		public bool IsList { get { return Type == PropertyType.list; } }
		/// <summary>
		/// Only if Type is a list
		/// </summary>
		public PropertyType CountType { get; set; }
		/// <summary>
		/// Only if Type is a list
		/// </summary>
		public PropertyType TypeOfList { get; set; }
	}

	public class PlyObjectHeader
	{
		public PlyObjectHeader()
		{
			this.Comments = new List<string>();
			this.Elements = new NamedList<ElementDescription>();
		}
		public PlyFormat Format { get; set; }
		public double Vesion { get; set; }
		public List<String> Comments { get; set; }
		public NamedList<ElementDescription> Elements { get; set; }

	}
	public static class ExtensionsMethods
	{
		public static string ReadLine(this Stream sr)
		{
			int byteCount;
			return ReadLine(sr, out byteCount);
		}
		public static string ReadLine(this Stream sr, out int byteCount)
		{
			String line = null;
			byteCount = 0;
			int c;
			while ((c = sr.ReadByte()) != -1)
			{
				byteCount++;
				if (c == 10 || c == 13) { break; }
				line += ((char)c);
			}
			long posB = sr.Position;
			c = sr.ReadByte();
			if (c == 10 || c == 13)
			{
				byteCount++;
			}
			else
			{
				sr.Position = posB;
			}
			return line;
		}
	}
	public class LoaderPlyModel : AbstractLoaderModel
	{
		public LoaderPlyModel()
			: base()
		{
		}
		public LoaderPlyModel(string path)
			: base(path)
		{
		}
		public override event ElementLoadEventHandler OnElementLoaded;
		public override Triangle[] Load()
		{
			this.ParserPlyModel();
			return this.triangles;
		}
		private void ParserPlyModel()
		{
			
			using (BufferedStream sr = new BufferedStream(File.OpenRead(this.path)))
			{
				int byteCount;
				PlyObjectHeader headerObj = this.GetPlyObjectHeader(sr, out byteCount);
				this.triangles = new Triangle[headerObj.Elements["face"].Count];
				sr.Position = byteCount;
				switch (headerObj.Format)
				{
					case PlyFormat.ascii:
						FileInfo info = new FileInfo(this.path);
						byte[] file = new byte[info.Length];
						sr.Read(file, 0, file.Length);
						string content = Encoding.ASCII.GetString(file);
						this.ParserASCII(headerObj, content);
						file = null;
						content = null;
						GC.Collect();
						GC.WaitForPendingFinalizers();
						
						//antes = DateTime.Now;
						//this.ParserASCII(headerObj, sr);
						//TimeSpan depoisNovo = antes - DateTime.Now;
						//System.Windows.Forms.MessageBox.Show("Tempo Novo: " + depoisNovo);


						break;
					case PlyFormat.binary_big_endian:
						using (EndianessBinaryReader br = new EndianessBinaryReader(sr, Endianess.BigEndian))
						{
							this.ParserBinary(headerObj, br);
						}
						break;
					case PlyFormat.binary_little_endian:
						using (EndianessBinaryReader br = new EndianessBinaryReader(sr, Endianess.LittleEndian))
						{
							this.ParserBinary(headerObj, br);
						}
						break;
					default:
						throw new FormatException("Invalid File format!");
				}
			}
		}

		private PlyObjectHeader GetPlyObjectHeader(Stream sr, out int byteCount)
		{
			PlyObjectHeader headerObj = new PlyObjectHeader();
			int lineByteCount;
			string line = ExtensionsMethods.ReadLine(sr, out lineByteCount);
			byteCount = lineByteCount;
			if (line.ToLower() != "ply")
			{
				throw new ArgumentException("Invalid file format. PLY indentifier expected in header file.");
			}
			String[] lineSplits;
			while (line.ToLower() != "end_header")
			{
				line = ExtensionsMethods.ReadLine(sr, out lineByteCount);
				byteCount += lineByteCount;
				lineSplits = line.Trim().Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
				switch (lineSplits[0])
				{
					case "format":
						lineSplits = lineSplits[1].Split(' ');
						headerObj.Format = (PlyFormat)Enum.Parse(typeof(PlyFormat), lineSplits[0]);
						NumberFormatInfo nfi = new NumberFormatInfo();
						nfi.NumberDecimalSeparator = ".";
						nfi.NumberGroupSeparator = ",";
						headerObj.Vesion = double.Parse(lineSplits[1], nfi);
						break;
					case "comment":
						headerObj.Comments.Add(lineSplits[1]);
						break;
					case "element":
						lineSplits = lineSplits[1].Split(' ');
						ElementDescription element =
							new ElementDescription(lineSplits[0].ToLower(), Convert.ToInt32(lineSplits[lineSplits.Length - 1]));
						headerObj.Elements.Add(element);
						break;
					case "property":
						lineSplits = lineSplits[1].Split(' ');
						PropertyType pType = (PropertyType)Enum.Parse(typeof(PropertyType), lineSplits[0]);
						String pName = lineSplits[lineSplits.Length - 1];
						PropertyDescription property = new PropertyDescription(pType, pName);
						if (pType == PropertyType.list)
						{
							property.CountType = (PropertyType)Enum.Parse(typeof(PropertyType), lineSplits[1]);
							property.TypeOfList = (PropertyType)Enum.Parse(typeof(PropertyType), lineSplits[2]);
						}
						headerObj.Elements[headerObj.Elements.Count - 1].Properties.Add(property);
						break;
				}
			}
			return headerObj;
		}
		private void ParserASCII(PlyObjectHeader header, String content)
		{
			StringReader sr = new StringReader(content);


			Point3D[] vertices = new Point3D[header.Elements["vertex"].Count]; ;
			PointerToVertex[] pointersToVertex = new PointerToVertex[header.Elements["face"].Count];
			//int headerLength = header.Count + 1;
			//while (sr.ReadLine().ToLower() != "end_header") { } //Pass header...
			NumberFormatInfo nfi = new NumberFormatInfo();
			nfi.NumberDecimalSeparator = ".";
			nfi.NumberGroupSeparator = ",";
			String[] str;

			//for number of vertices readed in header do...
			for (int i = 0; i < vertices.Length; i++)
			{

				str = sr.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				vertices[i] = new Point3D(double.Parse(str[0], nfi), double.Parse(str[1], nfi),
										  double.Parse(str[2], nfi));
				//Adjusting BoundBox...
				this.BoundBox.Include(vertices[i]);
				//Reporting progress
				int percent = (int)(((float)i / vertices.Length) * 100.0d);
				if ((percent % 20) == 0)
				{
					this.OnElementLoaded(percent, ElementMesh.Vertex);
				}
			}

			for (int i = 0; i < this.triangles.Length; i++)
			{
				str = sr.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				pointersToVertex[i] = new PointerToVertex(Int32.Parse(str[1], nfi), Int32.Parse(str[2], nfi),
														  Int32.Parse(str[3], nfi));
				this.triangles[i] = new Triangle(vertices[pointersToVertex[i].Vertex1],
												 vertices[pointersToVertex[i].Vertex2],
												 vertices[pointersToVertex[i].Vertex3]);
				int percent = (int)(((float)i / this.triangles.Length) * 100.0d);
				if ((percent % 20) == 0)
				{
					this.OnElementLoaded(percent, ElementMesh.Triangle);
				}
			}
			//System.Windows.Forms.MessageBox.Show("Degenerate Triangles: " + RemoveDegenerateTriangles(ref pointersToVertex));
			//RemoveUnusedVertices(ref vertices, pointersToVertex);

			this.ProcessNormalsPerVertex(pointersToVertex, vertices.Length);

			vertices = null;
			pointersToVertex = null;
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
		private void ParserASCII(PlyObjectHeader header, Stream sr)
		{
			Point3D[] vertices = new Point3D[header.Elements["vertex"].Count]; ;
			PointerToVertex[] pointersToVertex = new PointerToVertex[header.Elements["face"].Count];
			//int headerLength = header.Count + 1;
			//while (sr.ReadLine().ToLower() != "end_header") { } //Pass header...
			NumberFormatInfo nfi = new NumberFormatInfo();
			nfi.NumberDecimalSeparator = ".";
			nfi.NumberGroupSeparator = ",";
			String[] str;


			//for number of vertices readed in header do...
			for (int i = 0; i < vertices.Length; i++)
			{
				str = sr.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				vertices[i] = new Point3D(double.Parse(str[0], nfi), double.Parse(str[1], nfi),
										  double.Parse(str[2], nfi));
				//Adjusting BoundBox...
				this.BoundBox.Include(vertices[i]);
				//Reporting progress
				int percent = (int)(((float)i / vertices.Length) * 100.0d);
				if ((percent % 20) == 0)
				{
					this.OnElementLoaded(percent, ElementMesh.Vertex);
				}
			}

			for (int i = 0; i < this.triangles.Length; i++)
			{
				str = sr.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				pointersToVertex[i] = new PointerToVertex(Int32.Parse(str[1], nfi), Int32.Parse(str[2], nfi),
														  Int32.Parse(str[3], nfi));
				this.triangles[i] = new Triangle(vertices[pointersToVertex[i].Vertex1],
												 vertices[pointersToVertex[i].Vertex2],
												 vertices[pointersToVertex[i].Vertex3]);
				int percent = (int)(((float)i / this.triangles.Length) * 100.0d);
				if ((percent % 20) == 0)
				{
					this.OnElementLoaded(percent, ElementMesh.Triangle);
				}
			}
			//System.Windows.Forms.MessageBox.Show("Degenerate Triangles: " + RemoveDegenerateTriangles(ref pointersToVertex));
			//RemoveUnusedVertices(ref vertices, pointersToVertex);

			this.ProcessNormalsPerVertex(pointersToVertex, vertices.Length);

			vertices = null;
			pointersToVertex = null;
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
		private void ParserBinary(PlyObjectHeader header, BinaryReader ebr)
		{
			Point3D[] vertices = new Point3D[header.Elements["vertex"].Count]; ;
			PointerToVertex[] pointersToVertex = new PointerToVertex[header.Elements["face"].Count];
			for (int i = 0; i < header.Elements.Count; i++)
			{
				ElementDescription element = header.Elements[i];
				for (int e = 0; e < element.Count; e++)
				{
					for (int p = 0; p < element.Properties.Count; p++)
					{
						PropertyDescription property = element.Properties[p];
						if (property.IsList)
						{
							int listCount = 0;
							if (property.CountType == PropertyType.uchar || property.CountType == PropertyType.uint8)
							{
								listCount = ebr.ReadByte();
							}
							else
							{
								throw new ArgumentException("The type expected for List Count is 'uchar'. Type in file: '" + property.CountType + "'");
							}

							if (property.TypeOfList == PropertyType.@int || property.TypeOfList == PropertyType.int32)
							{
								if (element.Name == "face")
								{
									for (int j = 0; j < listCount; j += 3)
									{
										pointersToVertex[e].Vertex1 = ebr.ReadInt32();
										pointersToVertex[e].Vertex2 = ebr.ReadInt32();
										pointersToVertex[e].Vertex3 = ebr.ReadInt32();

										this.triangles[e] = new Triangle(vertices[pointersToVertex[e].Vertex1],
																		 vertices[pointersToVertex[e].Vertex2],
																		 vertices[pointersToVertex[e].Vertex3]);


										int percent = (int)(((float)e / this.triangles.Length) * 100.0d);
										if ((percent % 20) == 0)
										{
											this.OnElementLoaded(percent, ElementMesh.Triangle);
										}
									}
								}
								else if (element.Name == "vertex")
								{
									#region Ignore other properties for vertex
									//for (int j = 0; j < listCount; j += 3)
									//{
									//    ebr.ReadInt32();
									//    ebr.ReadInt32();
									//    ebr.ReadInt32();
									//}
									#endregion
									ebr.ReadBytes(element.Properties.SumTypesLength);
									break;
								}
							}
							else
							{
								throw new ArgumentException("The type expected for List elements is 'int'. Type in file: '" + property.TypeOfList + "'");
							}
						}
						else
						{
							if (element.Name == "face"){
								ebr.ReadBytes(element.Properties.SumTypesLength);
								break;
							}else if (element.Name == "vertex"){
								if (property.Name.ToLowerInvariant() == "x"){
									vertices[e].X = ebr.ReadSingle();
									vertices[e].Y = ebr.ReadSingle();
									vertices[e].Z = ebr.ReadSingle();
									p += (2 + element.Properties.Count - p);
									ebr.ReadBytes(element.Properties.SumTypesLength - 12);
									//Adjusting BoundBox...
									this.BoundBox.Include(vertices[e]);
									//Reporting progress
									int percent = (int)(((float)e / vertices.Length) * 100.0d);
									if ((percent % 20) == 0)
									{
										this.OnElementLoaded(percent, ElementMesh.Vertex);
									}
								}
								#region Antigo
								//switch (property.Type)
								//{
								//    case PropertyType.@char:
								//    case PropertyType.int8:
								//        ebr.ReadSByte();
								//        break;
								//    case PropertyType.uchar:
								//    case PropertyType.uint8:
								//        ebr.ReadByte();
								//        break;
								//    case PropertyType.@short:
								//    case PropertyType.int16:
								//        ebr.ReadInt16();
								//        break;
								//    case PropertyType.@ushort:
								//    case PropertyType.uint16:
								//        ebr.ReadUInt16();
								//        break;
								//    case PropertyType.@int:
								//    case PropertyType.int32:
								//        ebr.ReadInt32();
								//        break;
								//    case PropertyType.@uint:
								//    case PropertyType.uint32:
								//        ebr.ReadUInt32();
								//        break;
								//    case PropertyType.@float:
								//    case PropertyType.float32:
								//        vertices[e].X = ebr.ReadSingle();
								//        vertices[e].Y = ebr.ReadSingle();
								//        vertices[e].Z = ebr.ReadSingle();
								//        p += 2;
								//        //Adjusting BoundBox...
								//        this.BoundBox.Include(vertices[e]);
								//        //Reporting progress
								//        int percent = (int)(((float)e / vertices.Length) * 100.0d);
								//        if ((percent % 20) == 0)
								//        {
								//            this.OnElementLoaded(percent, ElementMesh.Vertex);
								//        }
								//        break;
								//    case PropertyType.@double:
								//    case PropertyType.float64:
								//        ebr.ReadDouble();
								//        break;
								//    default:
								//        break;
								//} 
								#endregion
							}
						}
					}
				}
			}
			this.ProcessNormalsPerVertex(pointersToVertex, vertices.Length);
			vertices = null;
			pointersToVertex = null;
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}
		protected void ProcessNormalsPerVertex(PointerToVertex[] pointersToVertex, int verticesCount)
		{
			Vector3D[] normalsPerVertex = new Vector3D[verticesCount];
			for (int i = 0; i < this.triangles.Length; i++)
			{
				normalsPerVertex[pointersToVertex[i].Vertex1] += this.triangles[i].Normal;
				normalsPerVertex[pointersToVertex[i].Vertex2] += this.triangles[i].Normal;
				normalsPerVertex[pointersToVertex[i].Vertex3] += this.triangles[i].Normal;

				int percent = (int)(((float)i / this.triangles.Length) * 100.0d);
				if ((percent % 20) == 0)
				{
					this.OnElementLoaded(percent, ElementMesh.VertexNormal);
				}
			}
			for (int i = 0; i < this.triangles.Length; i++)
			{
				this.triangles[i].NormalOnVertex1 = normalsPerVertex[pointersToVertex[i].Vertex1];
				this.triangles[i].NormalOnVertex1.Normalize();
				this.triangles[i].NormalOnVertex2 = normalsPerVertex[pointersToVertex[i].Vertex2];
				this.triangles[i].NormalOnVertex2.Normalize();
				this.triangles[i].NormalOnVertex3 = normalsPerVertex[pointersToVertex[i].Vertex3];
				this.triangles[i].NormalOnVertex3.Normalize();
				int percent = (int)(((float)i / this.triangles.Length) * 100.0d);
				if ((percent % 20) == 0)
				{
					this.OnElementLoaded(percent / 2 + 50, ElementMesh.VertexNormal);
				}
			}
			normalsPerVertex = null;
		}

		public override List<string> Extensions
		{
			get { return new List<string> { ".ply" }; }
		}
	}
}
